import requests
from dotenv import load_dotenv, find_dotenv
import os

class ApiService:
    def __init__(self):
        # Carrega as variáveis do arquivo .env
        load_dotenv(find_dotenv())
        self.api_token = os.getenv('API_TOKEN')
        if not self.api_token:
            raise ValueError("API_TOKEN não encontrado. Verifique seu arquivo .env")
        self.url = 'https://wdapi2.com.br/consulta'
        self._placa = None
        self._modelo = None
        self._ano = None
        self._marca = None

    def get_vehicle_info(self, placa):
        """Consulta informações do veículo na API."""
        link = f"{self.url}/{placa}/{self.api_token}"
        
        try:
            response = requests.get(link)
            response.raise_for_status()  # Levanta um erro para status HTTP 4xx/5xx
            
            data = response.json()
            if 'erro' in data:
                raise Exception(f"Erro na API: {data['erro']}")

            self._placa = placa
            self._modelo = data.get('modelo')
            self._ano = data.get('ano')
            self._marca = data.get('marca')
        
        except requests.exceptions.RequestException as e:
            raise Exception(f"Erro na requisição: {e}")

    def __str__(self):
        return f"Placa: {self.placa}, Modelo: {self.modelo}, Ano: {self.ano}, Marca: {self.marca}"

    @property
    def placa(self):
        return self._placa

    @property
    def modelo(self):
        return self._modelo

    @property
    def ano(self):
        return self._ano

    @property
    def marca(self):
        return self._marca
