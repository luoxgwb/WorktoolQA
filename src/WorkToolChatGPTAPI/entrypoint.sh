#!/bin/sh

# ע��: $PORT �ǳ���Ҫ, Railway ��������˶˿�
export ASPNETCORE_URLS="http://+:5033"
export ASPNETCORE_ENVIRONMENT="Production"
export TZ="Asia/Shanghai"

dotnet WorkToolChatGPTAPI.dll