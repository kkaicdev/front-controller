# front-controller

O padrão Front Controller é um padrão de arquitetura amplamente utilizado em aplicações web, especialmente em frameworks baseados em MVC (Model-View-Controller).
Seu principal objetivo é centralizar o controle de todas as requisições da aplicação em um único ponto antes que elas sejam processadas.

Em vez de cada servlet, controller ou endpoint tratar a requisição diretamente, todas as requisições passam primeiro por um único componente central, chamado Front Controller.

Esse componente é responsável por:

- Receber e interpretar as requisições do cliente (navegador, API, etc.);
- Executar tarefas comuns, como autenticação, autorização, logging e roteamento;
- Encaminhar a requisição ao controlador específico que irá tratar a lógica de negócio adequada.

---

Fluxo básico:

Cliente -> Front Controller -> Dispatcher -> Controller Específico -> View.

Esse padrão promove separação de responsabilidades, reduz duplicação de código e facilita a manutenção, pois concentra o ponto de entrada da aplicação web em um único local.

---
