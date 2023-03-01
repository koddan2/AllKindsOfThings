# https://developer.confluent.io/get-started/dotnet/#create-topic

podman-compose exec broker `
  kafka-topics --create `
    --topic purchases `
    --bootstrap-server localhost:9092 `
    --replication-factor 1 `
    --partitions 1