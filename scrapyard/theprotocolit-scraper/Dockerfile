FROM python:3.12-slim

RUN useradd -u 10001 -m app

WORKDIR /app

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY . .

USER app

EXPOSE 8802
CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8802"]
