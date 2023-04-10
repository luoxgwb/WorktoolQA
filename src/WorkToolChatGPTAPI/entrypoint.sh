#!/bin/sh

# 注意: $PORT 非常重要, Railway 必须监听此端口
export ASPNETCORE_URLS="http://+:5033"
export ASPNETCORE_ENVIRONMENT="Production"
export TZ="Asia/Shanghai"

dotnet WorkToolChatGPTAPI.dll