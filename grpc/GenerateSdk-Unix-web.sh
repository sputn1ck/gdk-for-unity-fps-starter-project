#!/bin/bash
cd proto
protoc -I=. service.proto --js_out=import_style=commonjs:../web --grpc-web_out=import_style=typescript,mode=grpcwebtext:../web
exit 0
