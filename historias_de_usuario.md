# Historias de Usuario — Happy Pets

## 👥 Roles
- **Secretaria** (Sec)
- **Doctor** (Doc)
- **Administrador** (Admin)

---

### HU-0: Login
* **Como:** Empleado (Sec/Doc/Admin)
* **Quiero:** Iniciar sesión con mi correo, contraseña y rol.
* **Para:** Acceder a mis funciones en el sistema.
* **Criterios:**
  1. Validar credenciales y rol.
  2. Generar token JWT.

---

### HU-1: Registro de Clientes y Mascotas
* **Como:** Secretaria
* **Quiero:** Registrar un cliente y su mascota.
* **Para:** Habilitar su historial clínico.
* **Criterios:**
  1. Guardar DNI, datos del cliente y datos básicos de la mascota.
  2. Validar que el DNI no esté duplicado.

---

### HU-2: Agendar Citas
* **Como:** Secretaria
* **Quiero:** Programar una cita para una mascota.
* **Para:** Reservar la atención con un doctor.
* **Criterios:**
  1. Validar disponibilidad del doctor en la fecha/hora.
  2. Guardar cita en estado "pendiente".

---

### HU-3: Consulta Médica
* **Como:** Doctor
* **Quiero:** Registrar el diagnóstico y productos usados.
* **Para:** Actualizar el historial y descontar del inventario.
* **Criterios:**
  1. Guardar detalles clínicos asociados a la cita.
  2. Reducir stock del producto seleccionado automáticamente.

---

### HU-4: Facturación con Tasa BCV
* **Como:** Secretaria
* **Quiero:** Cobrar una consulta a la tasa del día del BCV.
* **Para:** Generar el PDF y registrar el pago multidivisa (USD/VES).
* **Criterios:**
  1. Aplicar la tasa oficial actualizada del BCV.
  2. Crear factura y generar su archivo PDF.

---

### HU-5: Alerta de Stock Mínimo
* **Como:** Administrador
* **Quiero:** Recibir alertas de productos con bajo inventario.
* **Para:** Reponer insumos antes de que se agoten.
* **Criterios:**
  1. Disparar alerta si el stock baja del mínimo.
  2. Enviar notificación push en tiempo real.

---

### HU-6: Chat Interno
* **Como:** Empleado
* **Quiero:** Chatear en tiempo real con otros miembros de la clínica.
* **Para:** Coordinar la operación interna de forma ágil.
* **Criterios:**
  1. Envío/recepción instantánea de mensajes usando SignalR.

---

### HU-7: Reporte Mensual
* **Como:** Administrador
* **Quiero:** Exportar las ventas del mes en Excel o PDF.
* **Para:** Auditar el rendimiento económico.
* **Criterios:**
  1. Filtrar por rango de fechas.
  2. Restringir el acceso al endpoint solo a administradores.
