cd proto

..\bin\csharp\windows_x64\protoc.exe -I. -I../bin/csharp/windows_x64 -I../include/ -I%GOPATH%/src -I%GOPATH%/src/github.com/grpc-ecosystem/grpc-gateway/third_party/googleapis --csharp_out ../csharp --grpc_out ../csharp --plugin=protoc-gen-grpc=../bin/csharp/windows_x64/grpc_csharp_plugin.exe invoices.proto


cmd /k