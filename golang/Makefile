all: test vet fmt build

test:
	go test ./... -v

vet:
	go vet ./...

fmt:
	go list -f '{{.Dir}}' ./... | xargs -L1 gofmt -l
	# test -z $$(go list -f '{{.Dir}}' ./... | xargs -L1 gofmt -l)

build:
	go build -o bin/pkg ./pkg/chip8
	go build -o bin/main ./cmd/main
