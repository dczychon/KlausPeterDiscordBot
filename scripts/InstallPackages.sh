#!/bin/bash

if [[ $UID != 0 ]]; then
    echo "Please run this script with sudo: sudo $0 $*"
    exit 1
fi

wget -q -O /tmp/packages-microsoft-prod.deb https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb
dpkg -i /tmp/packages-microsoft-prod.deb
apt-get update
apt-get install dotnet-runtime-2.1 -y
apt-get install powershell -y