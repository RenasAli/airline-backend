#!/bin/bash

# Check if Docker is installed
if ! [ -x "$(command -v docker)" ]; then
  echo "Docker is not installed. Please install Docker and try again."
  exit 1
fi

# Variables
CONTAINER_NAME="neo4j-db-container"
NEO4J_VERSION="5.6"
NEO4J_USERNAME="neo4j"
NEO4J_PASSWORD="airline.123123"
HOST_PORT=7474  # Port for HTTP access to Neo4j browser
BOLT_PORT=7687  # Port for Bolt protocol connections

# Pull the Neo4j Docker image
echo "Pulling Neo4j Docker image version $NEO4J_VERSION..."
docker pull neo4j:$NEO4J_VERSION

# Check if the container already exists
if [ $(docker ps -a -q -f name=$CONTAINER_NAME) ]; then
  echo "Container with the name $CONTAINER_NAME already exists. Stopping and removing it..."
  docker stop $CONTAINER_NAME
  docker rm $CONTAINER_NAME
fi

# Run the Docker container
echo "Starting Neo4j container..."
docker run -d \
  --name $CONTAINER_NAME \
  -p $HOST_PORT:7474 \
  -p $BOLT_PORT:7687 \
  -e NEO4J_AUTH="$NEO4J_USERNAME/$NEO4J_PASSWORD" \
  neo4j:$NEO4J_VERSION

# Wait for the container to start
echo "Waiting for Neo4j container to initialize..."
sleep 15

# Check if the container is running
if [ $(docker ps -q -f name=$CONTAINER_NAME) ]; then
  echo "Neo4j is up and running!"
  echo "Access the Neo4j Browser at: http://localhost:$HOST_PORT"  
  echo "Bolt Protocol is available at: bolt://localhost:$BOLT_PORT"
  echo "Username: $NEO4J_USERNAME"
  echo "Password: $NEO4J_PASSWORD"
else
  echo "Failed to start Neo4j container. Check Docker logs for more details."
  docker logs $CONTAINER_NAME
fi