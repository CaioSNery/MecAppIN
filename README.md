# MecAppIN – Sistema de Oficina Mecânica (WPF)

Projeto em **WPF com C#**, simples, completo e **em produção**.

Este sistema foi desenvolvido inicialmente para ajudar meu pai, que há mais de **15 anos utilizava Excel** para controlar **notas, orçamentos e ordens de serviço** em sua oficina mecânica.  
O objetivo foi **centralizar tudo em um único software**, melhorar a organização, reduzir erros manuais e tornar o dia a dia mais rápido e confiável.

O sistema funciona **totalmente offline**, resolvendo um problema real do negócio e melhorando significativamente a experiência de uso.

---

## 📌 Funcionalidades

### 1️⃣ Ordem de Serviço (OS)
- Criação de **Ordem de Serviço**
- Possibilidade de **salvar como Orçamento** ou **imprimir diretamente**
- OS é salva automaticamente no **banco de dados**, em um caminho criado e controlado pelo próprio sistema
- Opção de **alternar entre Diesel e Gasolina**, atualizando os valores dinamicamente na tela
- Reimpressão de OS já existentes

---

### 2️⃣ Busca de Ordem de Serviço
- Busca rápida por OS
- Marcação de:
  - OS **pagas**
  - OS **a cobrar**
- Acesso direto à OS
- Possibilidade de:
  - Editar
  - Reimprimir
  - Atualizar status

---

### 3️⃣ Orçamentos
- Busca de orçamentos já criados
- Edição de orçamentos existentes
- Conversão de **Orçamento em Ordem de Serviço** com reaproveitamento dos dados

---

### 4️⃣ Clientes
- Cadastro de clientes com:
  - Nome
  - Telefone
  - Endereço
- Busca dinâmica de clientes ao criar uma OS
- Preenchimento automático dos dados do cliente na Ordem de Serviço

---

### 5️⃣ Financeiro
- Registro de **entradas e saídas** de valores
- Geração de **PDF financeiro diário**
- Tela de consulta financeira com:
  - Busca de OS por data
  - Soma de valores por:
    - Dia
    - Semana
    - Mês
    - Intervalo de datas personalizado
- Organização financeira simples e objetiva, pensada para o uso real no dia a dia da oficina

---

## 🗂️ PDFs
- Geração de PDFs para:
  - Ordens de Serviço
  - Orçamentos
  - Relatórios financeiros diários
- PDFs gerados localmente, sem dependência de internet

---

## 🧠 Tecnologias Utilizadas

- **C#**
- **.NET 8**
- **WPF (Windows Presentation Foundation)**
- **MVVM**
- **Entity Framework Core**
- **SQLite** (banco de dados local)
- **QuestPDF** (geração de PDFs)
- **Single File Publish** para distribuição

---

## ⚙️ Arquitetura e Conceitos
- Separação por camadas (Models, ViewModels, Data, Views)
- Inicialização controlada do banco de dados
- Sistema resiliente a falhas de banco
- Execução offline
- Persistência local segura

---

## 🚀 Status do Projeto

✅ **Em produção**  
✅ Utilizado diariamente em ambiente real  
✅ Sistema estável  
✅ Resolvido problema real de negócio  

---

## 🎯 Objetivo do Projeto

Além de resolver um problema real da oficina, este projeto também serviu como:
- Consolidação prática de **WPF + MVVM**
- Uso real de **SQLite em produção**
- Geração de PDFs
- Controle financeiro
- Experiência completa de desenvolvimento, entrega e manutenção de software

---


## 📄 Licença
Projeto de uso privado, desenvolvido sob demanda real.
