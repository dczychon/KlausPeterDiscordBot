#!/bin/bash

if [[ $UID != 0 ]]; then
    echo "Please run this script with sudo: sudo $0 $*"
    exit 1
fi

servicename=klauspeterbot
serviceroot=/usr/share/klauspeterbot/
serviceuser=botuser

if [ ! -d $(dirname $0)/bin ]; then
    echo "bin directory does not exists!"
    exit 1
fi

if ! id $serviceuser >/dev/null 2>&1; then
    echo "$serviceuser must be created first"
    exit 1
fi

if [ -f /etc/systemd/system/$servicename.service ]; then
    echo "Stopping existing bot to perform update"
    systemctl stop $servicename
    systemctl disable $servicename
else
    echo "This is an install. Bot service does not exists"
fi

if [ ! -d $serviceroot ]; then
    echo "Creating service directory at $serviceroot"
    mkdir $serviceroot
    chown $serviceuser $serviceroot
else
    echo "Cleaning service directory at $serviceroot"
    rm -rf $serviceroot*
fi

cp -f -R $(dirname $0)/bin/publish/* $serviceroot
cp $(dirname $0)/bin/$servicename.service /etc/systemd/system
systemctl enable $servicename
systemctl start $servicename

sleep 5
if ! systemctl status $servicename |grep running >/dev/null 2>&1; then
    echo "FAILED. Bot is NOT running after update"
    exit 1
else
    echo "Successful. Bot is running"
    exit 0
fi
