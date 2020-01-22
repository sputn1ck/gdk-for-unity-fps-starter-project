

cd proto
protoc -I. -I../include/ -I%GOPATH%/src -I%GOPATH%/src/github.com/grpc-ecosystem/grpc-gateway/third_party/googleapis --go_out=plugins=grpc:../../app/delivery/grpc service.proto
protoc -I. -I../include/ -I%GOPATH%/src -I%GOPATH%/src/github.com/grpc-ecosystem/grpc-gateway/third_party/googleapis --grpc-gateway_out=logtostderr=true,grpc_api_configuration=service.yaml:../../app/delivery/grpc service.proto


cmd /k