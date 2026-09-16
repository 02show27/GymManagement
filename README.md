#  GymManagement - Sistema de Gestión de Gimnasios con Clases Grupales

Aplicacion web integral orientada al control transaccional de suscripciones, reservas de cupos con control estricto, generación de pases digitales con código QR y analítica ejecutiva en tiempo real.

---

##  Información del Proyecto

* **Programa:** Diplomado Full Stack 2026 (Módulo 6: SIG-116 // CENEFCO -  Virtual)
* **Docente:** Lic. Rodrigo Alave Chapi
* **Estudiante:** Jhonny Tito Castro
* **Ubicación:** La Paz - Bolivia

---

##  Arquitectura de Software

El sistema está diseñado bajo la **Arquitectura Onion (Cebolla)** desacoplada, garantizando la independencia del dominio del negocio de frameworks y bibliotecas externas:

* **`1.Domain`**: Entidades puras C# (`Socio`, `Reserva`, `HorarioClase`), enums y contratos base del núcleo sin dependencias externas.
* **`2.Application`**: Casos de uso (CQRS) basados en comandos (`CrearReservaCommand`, `CancelarReservaCommand`) y consultas.
* **`3.Infrastructure`**: Persistencia con Entity Framework Core (SQL Server), generación de pases QR (`QRCoder`) y reportes financieros PDF (`QuestPDF`).
* **`4.Presentation.Web`**: Interfaz de usuario basada en **ASP.NET Core MVC** y comunicación en tiempo real con SignalR (`OcupacionHub`).

---

##  Características Principales

* **Control Estricto de Reservas:** Las primeras N solicitudes reciben confirmación inmediata. Solicitudes posteriores se asignan a una cola de Lista de Espera .
* **Promoción Automática:** Al procesar una cancelación oportuna (hasta 2 horas antes de la clase), el usuario #1 ingresa a la lista de espera ascendente de forma automática a estado **CONFIRMADA**.
* **Pases Digitales QR:** Generación automática de código QR para validación de acceso dinámico en la recepción.
* **Pagos & Subscripciones:** Integración para membresías mensuales recurrentes y cobro de pases sueltos individuales.
* **Reportes Financieros:** Motor de exportación en PDF para finanzas y métricas de inasistencia (*no-show rate*).

---

##  Tecnologías Utilizadas

* **Lenguaje:** C# (.NET 8.0)
* **Framework Web:** ASP.NET Core MVC & SignalR
* **ORM:** Entity Framework Core 8.0 (SQL Server)
* **Patrones & Librerías:** CQRS (MediatR), QRCoder, QuestPDF, Bootstrap 5

---

##  Instrucciones de Ejecución Local

### Prerrequisitos
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* Servidor de Base de Datos SQL Server (LocalDB o instancia remota)

### Pasos
1. **Clonar el repositorio:**
   ```bash
   git clone [https://github.com/02show27/GymManagement.git](https://github.com/02show27/GymManagement.git)
   cd GymManagement