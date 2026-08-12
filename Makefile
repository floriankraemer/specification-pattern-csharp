.PHONY: help build up down shell restore test format format-check run-example check all

# Default target
help: ## Show this help message
	@echo 'Usage: make [target]'
	@echo ''
	@echo 'Targets:'
	@awk 'BEGIN {FS = ":.*?## "} /^[a-zA-Z_-]+:.*?## / {printf "  \033[36m%-15s\033[0m %s\n", $$1, $$2}' $(MAKEFILE_LIST)

# Docker commands
build: ## Build the dotnet container image
	docker compose build

up: ## Start the dotnet container
	docker compose up -d

down: ## Stop the dotnet container
	docker compose down

shell: ## Login to the dotnet container
	docker compose exec dotnet bash

# Development commands
restore: up ## Restore NuGet dependencies
	docker compose exec dotnet dotnet restore

test: up restore ## Run the xUnit test suite
	docker compose exec dotnet dotnet test

format: up restore ## Apply dotnet format code style fixes
	docker compose exec dotnet dotnet format

format-check: up restore ## Check code style without making changes (dotnet format)
	docker compose exec dotnet dotnet format --verify-no-changes

run-example: up restore ## Run the ECommerce demo
	docker compose exec dotnet dotnet run --project examples/ECommerce/ECommerce.csproj

run-example-accounting: up restore ## Run the Open-Item Accounting / Dunning demo
	docker compose exec dotnet dotnet run --project examples/OpenItemAccounting/OpenItemAccounting.csproj

run-example-accounting-german: up restore ## Run the German Open-Item Accounting / Dunning demo
	docker compose exec dotnet dotnet run --project examples/OpenItemAccountingGerman/OpenItemAccountingGerman.csproj

# Combined commands
check: format-check ## Run code style checks (dotnet's built-in analyzers run as part of `build`)

all: test check ## Run all tests and checks
