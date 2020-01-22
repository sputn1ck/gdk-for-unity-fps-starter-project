cd proto

protoc -I.  -I../include/ -I%GOPATH%/src -I%GOPATH%/src/github.com/grpc-ecosystem/grpc-gateway/third_party/googleapis --js_out=import_style=commonjs+dts:../web --grpc-web_out=import_style=commonjs+dts,mode=grpcwebtext:../web --plugin=protoc-gen-grpc-web=../bin/web/protoc-gen-grpc-web-1.0.4-windows-x86_64.exe service.proto


cmd /k