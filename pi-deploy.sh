#!/bin/bash

# pi-deploy.sh - Deploy Puck Drop to Raspberry Pi
#
# ===============================================================================
# RASPBERRY PI INITIAL SETUP (run these commands on your Pi BEFORE first deployment):
# ===============================================================================
#
# 1. Update system
# sudo apt update && sudo apt upgrade -y
#
# 2. Install essentials
# sudo apt install -y curl wget git htop nano
#
# 3. Create directories
# mkdir -p /home/admin/puckdrop
# mkdir -p /home/admin/logs
#
# NOTE: .NET does NOT need to be installed on the Pi!
# This script uses self-contained deployment which bundles the .NET runtime.
#
# 4. Optional but recommended: Set up SSH keys for passwordless deployment
#    On your local machine, run: ssh-copy-id admin@YOUR_PI_IP
#
# ===============================================================================
# DEPLOYMENT SCRIPT CONFIGURATION:
# ===============================================================================

# Configuration for your specific Pi
PI_USER="YOUR_PI_USER"             # Pi username (e.g., pi, admin)
PI_HOST="YOUR_PI_IP_HERE"          # Replace with your Pi's IP (e.g., 192.168.1.100)
PI_NAME="nhl"                      # Friendly name for your Pi
PI_PATH="/home/YOUR_PI_USER/puckdrop"  # Path where app will be deployed
PI_PORT="5050"                     # Port for the web app

# Validate configuration
if [ "$PI_HOST" = "YOUR_PI_IP_HERE" ]; then
    echo "❌ Please update PI_HOST with your Pi's IP address before running!"
    echo "   Edit this script and change PI_HOST=\"YOUR_PI_IP_HERE\" to your actual IP"
    echo "   Example: PI_HOST=\"192.168.1.100\""
    exit 1
fi

echo "🏒 Deploying Puck Drop to '$PI_NAME' Raspberry Pi..."
echo "📍 Target: $PI_USER@$PI_HOST:$PI_PORT"
echo "📂 App structure: .NET 10 self-contained deployment (no .NET required on Pi)"
echo ""
echo "⚠️  You will be prompted for the Pi password TWICE:"
echo "   1. When copying files (scp)"
echo "   2. When executing remote commands (ssh)"
echo ""
read -p "Press Enter to continue, or Ctrl+C to cancel..."

# Build and publish the .NET application
echo "🔨 Building .NET application for linux-arm64..."
cd .

# Clean previous builds
dotnet clean

# Publish self-contained for ARM64
dotnet publish -c Release -r linux-arm64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=false -o ./publish

if [ $? -ne 0 ]; then
    echo "❌ .NET publish failed"
    exit 1
fi

echo "✅ .NET application built successfully"

# Copy the database if it exists
if [ -f "nhl.db" ]; then
    echo "✅ Including existing database (nhl.db)"
    cp nhl.db ./publish/
fi

# Clean up any nested publish directory
if [ -d "./publish/publish" ]; then
    echo "🧹 Cleaning up nested publish directory..."
    rm -rf ./publish/publish
fi

# Create deployment package
echo "📋 Creating deployment package..."
cd publish
tar -czf ../puckdrop-deploy.tar.gz .
cd ..

# Copy to Pi
echo ""
echo "📤 Copying to $PI_NAME Pi..."
echo "🔐 Enter password when prompted:"
scp puckdrop-deploy.tar.gz $PI_USER@$PI_HOST:/tmp/

# Deploy and setup on Pi
echo ""
echo "🔧 Setting up on $PI_NAME Pi..."
echo "🔐 Enter password again when prompted:"
ssh $PI_USER@$PI_HOST << ENDSSH

# Stop any existing service
sudo systemctl stop puckdrop 2>/dev/null || true

# Create and setup app directory
mkdir -p $PI_PATH
mkdir -p /home/$PI_USER/logs
cd $PI_PATH

# Extract files
echo "📦 Extracting deployment package..."
tar -xzf /tmp/puckdrop-deploy.tar.gz
rm /tmp/puckdrop-deploy.tar.gz

# Debug: List all extracted files
echo "🔍 Files after extraction:"
ls -la

# Check if there's a nested publish directory
if [ -d "publish" ]; then
    echo "⚠️ Found nested publish directory, moving files up..."
    mv publish/* . 2>/dev/null || true
    mv publish/.* . 2>/dev/null || true
    rm -rf publish
fi

# Make the executable actually executable
chmod +x PuckDrop

# Verify files exist after extraction
if [ ! -f "PuckDrop" ]; then
    echo "❌ PuckDrop executable not found after extraction!"
    ls -la
    exit 1
fi

# Check database status
if [ -f "nhl.db" ]; then
    echo "✅ Database (nhl.db) deployed successfully"
else
    echo "📝 No existing database - will be created on first run"
fi

echo "✅ Application files verified on Pi"

# Create systemd service
echo "📝 Creating systemd service..."
sudo tee /etc/systemd/system/puckdrop.service > /dev/null << 'EOF'
[Unit]
Description=Puck Drop Web Service
After=network.target

[Service]
Type=simple
User=\$PI_USER
Group=\$PI_USER
WorkingDirectory=\$PI_PATH
ExecStart=\$PI_PATH/PuckDrop --urls "http://*:\$PI_PORT"
Restart=always
RestartSec=10
SyslogIdentifier=puckdrop
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

# Resource limits
LimitNOFILE=65536
MemoryMax=300M

[Install]
WantedBy=multi-user.target
EOF

# Fix the service file to use actual variables
sudo sed -i "s|\\\$PI_USER|$PI_USER|g" /etc/systemd/system/puckdrop.service
sudo sed -i "s|\\\$PI_PATH|$PI_PATH|g" /etc/systemd/system/puckdrop.service
sudo sed -i "s|\\\$PI_PORT|$PI_PORT|g" /etc/systemd/system/puckdrop.service

# Reload systemd and start service
echo "🚀 Starting NHL Monitor service..."
sudo systemctl daemon-reload
sudo systemctl enable puckdrop
sleep 2
sudo systemctl start puckdrop

# Wait for startup
echo "⏳ Waiting for service to start..."
sleep 5

# Check service status
if systemctl is-active --quiet puckdrop; then
    echo "✅ NHL Monitor service is running!"
else
    echo "❌ Service failed to start. Checking logs..."
    sudo systemctl status puckdrop --no-pager -l
    sudo journalctl -u puckdrop --no-pager -l -n 20
    exit 1
fi

# Show status
echo ""
echo "📊 Service Status:"
sudo systemctl status puckdrop --no-pager -l

echo ""
echo "📋 Recent logs:"
sudo journalctl -u puckdrop --no-pager -l -n 10

echo ""
echo "✅ Deployment complete on '$PI_NAME' Pi!"
echo "🌐 NHL Monitor is running at: http://$PI_HOST:$PI_PORT"
echo ""
echo "📋 Management commands:"
echo "   sudo systemctl status puckdrop    - Check service status"
echo "   sudo journalctl -u puckdrop -f    - Follow logs"
echo "   sudo systemctl restart puckdrop   - Restart service"
echo "   sudo systemctl stop puckdrop      - Stop service"

ENDSSH

# Clean up local temp file
rm puckdrop-deploy.tar.gz

echo ""
echo "🎉 NHL Monitor deployed successfully to '$PI_NAME' Pi!"
echo "🔗 Access your app at: http://$PI_HOST:$PI_PORT"
echo ""
echo "📋 SSH to your Pi:"
echo "   ssh $PI_USER@$PI_HOST"
echo ""
echo "💡 Tip: Set up SSH keys to avoid password prompts in future deployments!"
echo ""
