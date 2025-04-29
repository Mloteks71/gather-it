host="$1"
port="$2"
shift 2

timeout=15

echo "⏳ Waiting for $host:$port for $timeout seconds..."

for i in $(seq $timeout); do
  if nc -z "$host" "$port"; then
    echo "✅ $host:$port is available!"
    exec "$@"
    exit 0
  fi
  sleep 1
done

echo "❌ Timeout reached: $host:$port not available"
exit 1
