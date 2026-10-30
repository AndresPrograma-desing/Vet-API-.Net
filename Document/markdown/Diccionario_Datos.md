# Diccionario de Datos — Happy Pets (vet-api-Net)

> Documentación de cada tabla de la base de datos: propósito, columnas y relaciones.
> Orden alineado con los diagramas DER (`Document/diagramas/Drawio/`): Catálogo de Productos e Inventario → Gestión de Clientes y Mascotas → Agenda de Citas y Consultas Médicas → Facturación y Cobranza → Control de Vacunación → Usuarios, Roles y Permisos → Mensajería y Alertas Internas → Infraestructura del Sistema.
> **Nota de nombres:** en la interfaz (UI) la tabla `facturas` se muestra como **"Recibo"** y `detalles_factura` como **"Detalle de Recibo"** — el nombre técnico de la tabla/modelo en el backend no cambia.

---

## 1. Catálogo de Productos e Inventario

### `categorias_productos`

Clasifica los productos del sistema (ej. medicamentos, alimentos, accesorios).

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `nombre` | Nombre de la categoría | — |
| `descripcion` | Detalle opcional de la categoría | — |
| `activo` | Si la categoría está habilitada para uso | — |
| `creado` | Fecha de creación | — |
| `actualizado` | Fecha de última modificación | — |

### `productos`

Catálogo de productos vendibles o usados en consultas (medicamentos, insumos, alimentos).

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `categoria_id` | Categoría a la que pertenece | 1:N → `categorias_productos` (N productos por categoría) |
| `codigo` | Código interno/SKU | — |
| `nombre` | Nombre del producto | — |
| `descripcion` | Detalle del producto | — |
| `tipo` | Tipo de producto (medicamento, alimento, etc.) | — |
| `precio` | Precio de costo | — |
| `precio_venta` | Precio de venta al cliente | — |
| `stock` | Cantidad disponible actual | — |
| `stock_minimo` | Umbral para alertas de reposición | — |
| `unidad_medida` | Unidad de medida (ml, unidad, kg...) | — |
| `proveedor` | Proveedor del producto | — |
| `requiere_receta` | Si necesita prescripción veterinaria | — |
| `creado` / `actualizado` | Fechas de auditoría | — |

### `historial_precios`

Registra cada cambio de precio de un producto, para trazabilidad.

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `producto_id` | Producto cuyo precio cambió | 1:N → `productos` (un producto tiene muchos cambios históricos) |
| `precio_anterior` | Precio antes del cambio | — |
| `precio_nuevo` | Precio después del cambio | — |
| `usuario_id` | Quién hizo el cambio | 1:N → `usuarios` (un usuario puede registrar muchos cambios) |
| `motivo` | Razón del cambio de precio | — |
| `fecha_cambio` | Cuándo ocurrió el cambio | — |

### `movimientos_inventario`

Registra entradas y salidas de stock de productos (auditoría de inventario).

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `producto_id` | Producto afectado | 1:N → `productos` (un producto tiene muchos movimientos) |
| `tipo_movimiento` | Entrada, salida, ajuste, etc. | — |
| `cantidad` | Cantidad movida | — |
| `stock_anterior` | Stock antes del movimiento | — |
| `stock_nuevo` | Stock después del movimiento | — |
| `motivo` | Razón del movimiento | — |
| `referencia_id` | ID de referencia (ej. un recibo o consulta que lo originó) | — |
| `usuario_id` | Quién ejecutó el movimiento | 1:N → `usuarios` (un usuario registra muchos movimientos) |
| `fecha_movimiento` | Cuándo ocurrió | — |

**Siguiente:** `productos` continúa en `consultas_productos` (sección Agenda de Citas y Consultas Médicas).

---

## 2. Gestión de Clientes y Mascotas

### `especies`

Catálogo de especies de animales atendidas (perro, gato, etc.).

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `nombre` | Nombre de la especie | — |

### `clientes`

Dueños/responsables de las mascotas atendidas en la veterinaria.

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `nombre` / `apellido` | Nombre completo del cliente | — |
| `email` | Correo de contacto | — |
| `telefono` | Teléfono de contacto | — |
| `direccion` | Dirección física | — |
| `identificacion` | Documento de identidad | — |
| `nota` | Observaciones internas sobre el cliente | — |
| `creado` / `actualizado` | Fechas de auditoría | — |
| `image_url` | Foto o avatar del cliente | — |
| `PortalLoginCode` | Código temporal de acceso al portal de clientes | — |
| `PortalLoginCodeExpireDate` | Expiración de ese código | — |

### `mascotas`

Animales atendidos, cada uno perteneciente a un cliente.

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `cliente_id` | Dueño de la mascota | 1:N → `clientes` (un cliente puede tener varias mascotas) |
| `especie_id` | Especie de la mascota | 1:N → `especies` (una especie agrupa muchas mascotas) |
| `nombre` | Nombre de la mascota | — |
| `raza` | Raza | — |
| `color` | Color del pelaje/piel | — |
| `sexo` | Sexo de la mascota | — |
| `fecha_nacimiento` | Fecha de nacimiento | — |
| `peso` | Peso actual | — |
| `identeficacion_mascota` | Identificación (chip, placa, etc.) | — |
| `alergias` | Alergias conocidas | — |
| `condiciones_medicas` | Condiciones médicas preexistentes | — |
| `esterilizado` | Si está esterilizado/a | — |
| `creado` / `actualizado` | Fechas de auditoría | — |
| `image_url` | Foto de la mascota | — |
| `last_data_modified_at` | Última modificación de datos clínicos | — |
| `last_birthday_notified_year` | Último año en que se notificó el cumpleaños | — |

### `historias_clinicas`

Resumen clínico consolidado de una mascota, incluyendo análisis asistido por IA.

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `mascota_id` | Mascota a la que pertenece | 1:1 → `mascotas` (una historia clínica por mascota) |
| `resumen_ia` | Resumen generado por IA | — |
| `alertas_riesgo_ia` | Alertas de riesgo detectadas por IA | — |
| `sugerencias_ia` | Sugerencias generadas por IA | — |
| `notas_veterinario` | Notas manuales del veterinario | — |
| `ultimo_analisis_ia` | Fecha del último análisis de IA | — |
| `creado` / `actualizado` | Fechas de auditoría | — |

**Siguiente:** `mascotas` continúa como `mascota_id` en `citas` (sección Agenda de Citas y Consultas Médicas).

---

## 3. Agenda de Citas y Consultas Médicas

### `metodos_pago`

Catálogo de métodos de pago aceptados.

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `nombre` | Nombre del método (efectivo, tarjeta, transferencia...) | — |
| `creado` / `actualizado` | Fechas de auditoría | — |

### `servicios`

Catálogo de servicios ofrecidos (consulta general, cirugía, baño, etc.).

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `nombre` | Nombre del servicio | — |
| `descripcion` | Detalle del servicio | — |
| `precio` | Precio del servicio | — |
| `activo` | Si está disponible actualmente | — |
| `creado` / `actualizado` | Fechas de auditoría | — |

### `citas`

Agenda de citas programadas entre una mascota y el personal veterinario.

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `mascota_id` | Mascota citada | 1:N → `mascotas` (una mascota puede tener muchas citas) |
| `doctor_id` | Veterinario asignado | 1:N → `usuarios` (un doctor atiende muchas citas) |
| `secretaria_id` | Secretaria que agendó la cita | 1:N → `usuarios` (una secretaria agenda muchas citas) |
| `fecha_cita` / `hora_cita` | Fecha y hora programada | — |
| `motivo` | Motivo de la cita | — |
| `tipo_cita` | Tipo de cita (control, urgencia, etc.) | — |
| `estado` | Estado actual (pendiente, completada, cancelada...) | — |
| `notas` | Notas adicionales | — |
| `metodo_pago_id` | Método de pago previsto | 1:N → `metodos_pago` (un método de pago aplica a muchas citas) |
| `servicio_id` | Servicio asociado | 1:N → `servicios` (un servicio aplica a muchas citas) |

### `consultas`

Registro clínico de la atención efectiva derivada de una cita.

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `cita_id` | Cita que originó la consulta | 1:1 / 1:N → `citas` (una cita suele derivar en una consulta) |
| `mascota_id` | Mascota atendida | 1:N → `mascotas` (una mascota tiene muchas consultas) |
| `doctor_id` | Veterinario que atendió | 1:N → `usuarios` (un doctor realiza muchas consultas) |
| `fecha_consulta` | Fecha de la consulta | — |
| `peso_actual` | Peso registrado en esa consulta | — |
| `temperatura` | Temperatura registrada | — |
| `sintomas` | Síntomas reportados | — |
| `diagnostico` | Diagnóstico del veterinario | — |
| `tratamiento` | Tratamiento indicado | — |
| `receta` | Receta médica | — |
| `observaciones` | Observaciones adicionales | — |
| `ConsultaPrice` | Precio cobrado por la consulta | — |
| `creado` | Fecha de registro | — |
| `url_docx` | Documento Word generado para esta consulta | — |

### `consultas_productos`

Detalle de productos/medicamentos aplicados o entregados durante una consulta.

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `consulta_id` | Consulta en la que se usó el producto | 1:N → `consultas` (una consulta puede usar varios productos) |
| `producto_id` | Producto utilizado | 1:N → `productos` (un producto se usa en muchas consultas) |
| `cantidad` | Cantidad utilizada | — |
| `precio_unitario` | Precio unitario cobrado | — |
| `dosis` | Dosis aplicada | — |
| `via_administracion` | Vía de administración (oral, inyectable...) | — |
| `frecuencia` | Frecuencia de aplicación | — |
| `duracion` | Duración del tratamiento | — |
| `instrucciones` | Instrucciones para el cliente | — |
| `aplicado_por` | Quién aplicó el producto | 1:N → `usuarios` (un usuario aplica en muchos registros) |
| `creado` | Fecha de registro | — |

**Siguiente:** `consultas` continúa como `consulta_id` en `facturas`/Recibo (sección Facturación y Cobranza).

---

## 4. Facturación y Cobranza

### `facturas` (Recibo)

Comprobante de cobro generado para un cliente. En la interfaz del sistema se muestra como **"Recibo"**.

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `numero_factura` | Número de comprobante | — |
| `cliente_id` | Cliente al que se le emite el recibo | 1:N → `clientes` (un cliente tiene muchos recibos) |
| `mascota_id` | Mascota asociada al recibo | 1:N → `mascotas` (una mascota genera muchos recibos) |
| `consulta_id` | Consulta que originó el recibo | 1:N → `consultas` (una consulta puede generar varios recibos, ej. parciales) |
| `secretaria_id` | Quién emitió el recibo | 1:N → `usuarios` (un usuario emite muchos recibos) |
| `fecha_emision` | Fecha de emisión | — |
| `subtotal` | Subtotal antes de descuentos | — |
| `descuento` | Descuento aplicado | — |
| `total` | Total final | — |
| `metodo_pago` | Método de pago usado | — |
| `estado_pago` | Estado del pago (pagado, pendiente...) | — |
| `notas` | Notas adicionales | — |
| `url_docx` | Documento generado del recibo | — |
| `creado` / `actualizado` | Fechas de auditoría | — |

### `detalles_factura` (Detalle de Recibo)

Líneas de detalle de un recibo (ítems cobrados).

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `factura_id` | Recibo al que pertenece | 1:N → `facturas` (un recibo tiene muchas líneas de detalle) |
| `producto_id` | Producto cobrado (si aplica) | 1:N → `productos` (un producto aparece en muchos detalles) |
| `productos_consultas_id` | Línea de producto de consulta cobrada (si aplica) | 1:N → `consultas_productos` |
| `vaccine_id` | Vacuna cobrada (si aplica) | 1:N → `vaccines` |
| `servicio_id` | Servicio cobrado (si aplica) | 1:N → `servicios` |
| `descripcion` | Descripción libre del ítem | — |
| `cantidad` | Cantidad cobrada | — |
| `precio_unitario` | Precio unitario | — |
| `total` | Total de la línea | — |
| `created` | Fecha de creación | — |

**Siguiente:** el detalle del recibo continúa como `detalle_factura_id` en `pet_vaccinations` (sección Control de Vacunación).

---

## 5. Control de Vacunación

### `vaccines`

Catálogo de vacunas disponibles, específicas por especie.

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `name` | Nombre de la vacuna | — |
| `especie_id` | Especie para la que aplica | 1:N → `especies` (una especie tiene varias vacunas aplicables) |
| `minimum_age_weeks` | Edad mínima en semanas para aplicarla | — |
| `booster_frequency_value` / `booster_frequency_unit` | Frecuencia de refuerzo | — |
| `description` | Descripción de la vacuna | — |
| `price` | Precio de la vacuna | — |
| `active` | Si está activa/disponible | — |
| `created_at` | Fecha de creación | — |

### `vaccine_batches`

Lotes físicos de vacunas en stock, por laboratorio.

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `vaccine_id` | Vacuna a la que pertenece el lote | 1:N → `vaccines` (una vacuna tiene muchos lotes) |
| `laboratory` | Laboratorio fabricante | — |
| `batch_number` | Número de lote | — |
| `expiration_date` | Fecha de vencimiento | — |
| `quantity_in_stock` | Cantidad disponible de ese lote | — |
| `received_date` | Fecha de recepción | — |
| `active` | Si el lote está habilitado para uso | — |
| `created_at` | Fecha de creación | — |

### `vaccine_scheme_stages`

Define el esquema de dosis de una vacuna (ej. primera dosis, refuerzo a los 21 días).

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `vaccine_id` | Vacuna a la que pertenece la etapa | 1:N → `vaccines` (una vacuna tiene varias etapas de esquema) |
| `stage_type` | Tipo de etapa (inicial, refuerzo...) | — |
| `dose_number` | Número de dosis dentro del esquema | — |
| `interval_days_from_previous` | Días desde la dosis anterior | — |
| `minimum_age_weeks_override` | Edad mínima específica para esta etapa | — |
| `created_at` | Fecha de creación | — |

### `pet_vaccinations`

Registro de aplicación real de una vacuna a una mascota.

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `mascota_id` | Mascota vacunada | 1:N → `mascotas` (una mascota tiene muchas vacunaciones) |
| `vaccine_id` | Vacuna aplicada | 1:N → `vaccines` (una vacuna se aplica a muchas mascotas) |
| `vaccine_batch_id` | Lote específico utilizado | 1:N → `vaccine_batches` (un lote se usa en muchas aplicaciones) |
| `consulta_id` | Consulta en la que se aplicó | 1:N → `consultas` (una consulta puede incluir varias vacunaciones) |
| `doctor_id` | Veterinario que aplicó la vacuna | 1:N → `usuarios` (un doctor aplica muchas vacunas) |
| `application_date` | Fecha de aplicación | — |
| `weight_at_application` | Peso de la mascota al momento de aplicar | — |
| `dose` | Dosis aplicada | — |
| `next_dose_date` | Fecha estimada de la próxima dosis | — |
| `status` | Estado (aplicada, pospuesta, pendiente...) | — |
| `postponement_reason` | Motivo de postergación (si aplica) | — |
| `clinical_observations` | Observaciones clínicas | — |
| `detalle_factura_id` | Línea de recibo asociada | 1:N → `detalles_factura` (una línea de recibo puede corresponder a una vacunación) |
| `reminder_seven_sent_at` / `reminder_three_sent_at` / `reminder_day_before_sent_at` | Marcas de recordatorios enviados | — |
| `created_at` | Fecha de creación | — |

**Siguiente:** `doctor_id` (usuarios) conecta con la sección Usuarios, Roles y Permisos.

---

## 6. Usuarios, Roles y Permisos

### `usuarios`

Personal interno del sistema (administradores, doctores, secretarias).

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `nombre` / `apellido` | Nombre completo | — |
| `email` | Correo de acceso | — |
| `contraseña` | Contraseña (hash) | — |
| `PasswordRecoveryCode` / `CodeRecoveryExpireDate` | Código y expiración para recuperar contraseña | — |
| `rol` | Rol del usuario (admin, doctor, secretaria) | — |
| `activo` | Si la cuenta está habilitada | — |
| `ultimo_acceso` | Último inicio de sesión | — |
| `creado` / `actualizado` | Fechas de auditoría | — |
| `AvatarUrl` | Foto de perfil | — |
| `Telefono` | Teléfono de contacto | — |
| `external_auth_id` | ID de autenticación externa (si aplica) | — |

### `user_permissions`

Permisos personalizados asignados a un usuario, más allá de su rol base.

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `user_id` | Usuario al que pertenecen los permisos | 1:N → `usuarios` (un usuario tiene un set de permisos; históricamente puede tener más de un registro) |
| `permissions` | Permisos en formato JSON | — |
| `created_at` / `updated_at` | Fechas de auditoría | — |

### `permission_modules`

Módulos del sistema sobre los que se pueden otorgar permisos (ej. Facturación, Inventario).

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `module_key` | Clave interna del módulo | — |
| `label` | Nombre visible del módulo | — |
| `icon` | Ícono asociado | — |
| `sort_order` | Orden de visualización | — |
| `creado` | Fecha de creación | — |

### `permission_definitions`

Permisos específicos dentro de cada módulo (ej. "crear factura", "ver reportes").

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `module_id` | Módulo al que pertenece el permiso | 1:N → `permission_modules` (un módulo agrupa varios permisos) |
| `key` | Clave interna del permiso | — |
| `label` | Nombre visible del permiso | — |
| `sort_order` | Orden de visualización | — |
| `creado` | Fecha de creación | — |

### `password_reset_tickets`

Solicitudes de recuperación de contraseña generadas por un usuario.

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `usuario_id` | Usuario que solicitó el reset | 1:N → `usuarios` (un usuario puede generar varios tickets) |
| `token` | Token de recuperación | — |
| `estado` | Estado del ticket (pendiente, usado, expirado) | — |
| `expiracion` | Fecha de expiración | — |
| `creado` | Fecha de creación | — |

### `logs_sistema`

Auditoría de acciones realizadas por usuarios sobre el sistema.

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `usuario_id` | Usuario que ejecutó la acción | 1:N → `usuarios` (un usuario genera muchos logs) |
| `accion` | Acción realizada | — |
| `tabla_afectada` | Tabla sobre la que se actuó | — |
| `registro_id` | ID del registro afectado | — |
| `datos_previos` | Estado anterior del registro | — |
| `datos_nuevos` | Estado nuevo del registro | — |
| `ip_address` | IP desde donde se ejecutó | — |
| `user_agent` | Navegador/cliente usado | — |
| `created_at` | Fecha de creación | — |

**Siguiente:** `usuarios` opera todas las entidades anteriores — cierra el ciclo con el módulo de Productos e Inventario.

---

## 7. Mensajería y Alertas Internas

### `mensajes`

Mensajería directa entre usuarios del sistema (chat interno).

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `emisor_id` | Usuario que envía el mensaje | 1:N → `usuarios` (un usuario envía muchos mensajes) |
| `receptor_id` | Usuario que recibe el mensaje | 1:N → `usuarios` (un usuario recibe muchos mensajes) |
| `contenido` | Texto del mensaje | — |
| `leido` | Si fue leído | — |
| `fecha_envio` | Fecha de envío | — |

### `alertas_internas`

Notificaciones y alertas internas dirigidas a roles o usuarios específicos.

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `titulo` | Título de la alerta | — |
| `mensaje` | Contenido de la alerta | — |
| `tipo` | Tipo de alerta | — |
| `prioridad` | Prioridad (baja, media, alta...) | — |
| `destinatario_rol` | Rol destinatario (si aplica) | — |
| `destinatario_id` | Usuario destinatario específico | 1:N → `usuarios` (un usuario puede recibir muchas alertas) |
| `referencia_tabla` / `referencia_id` | Tabla/registro relacionado con la alerta | — |
| `accion_requerida` | Si requiere una acción del usuario | — |
| `fecha_limite` | Fecha límite para resolverla | — |
| `completada` | Si fue resuelta | — |
| `fecha_completado` | Fecha de resolución | — |
| `leida` | Si fue leída | — |
| `fecha_lectura` | Fecha de lectura | — |
| `leida_por` | Usuario que la marcó como leída | 1:N → `usuarios` (un usuario puede marcar muchas alertas como leídas) |
| `created_at` | Fecha de creación | — |

---

## 8. Infraestructura del Sistema

> Tablas de infraestructura y soporte técnico, en su mayoría independientes entre sí. No forman parte del flujo narrativo de negocio.

### `system_configs`

Configuración global del sistema (URLs, credenciales de servicios externos).

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `frontend_url` / `backend_external_url` | URLs públicas del sistema | — |
| `last_updated` | Última modificación | — |
| `bcv_api_url` | URL del servicio de tasa de cambio BCV | — |
| `email_provider_api_key` / `email_provider_api_url` / `email_provider_from_email` | Configuración del proveedor de emails | — |

### `EmailTemplates`

Plantillas HTML usadas para el envío de correos del sistema.

| Columna | Descripción | Relación |
|---|---|---|
| `Id` | Identificador único | — |
| `HtmlCode` | Contenido HTML de la plantilla | — |
| `TypeEmail` | Tipo de email (bienvenida, recordatorio, etc.) | — |
| `Update` / `CreatedAt` | Fechas de auditoría | — |

### `MoneyTypes`

Historial de tasas de cambio de moneda (ej. dólar BCV).

| Columna | Descripción | Relación |
|---|---|---|
| `Id` | Identificador único | — |
| `MoneyName` | Nombre de la moneda | — |
| `BcvDollar` | Valor de la tasa | — |
| `DollarPersistence` | Dato de persistencia del valor | — |
| `Fecha` | Fecha de la tasa | — |

### `Reportes`

Reportes generados y guardados por los usuarios del sistema.

| Columna | Descripción | Relación |
|---|---|---|
| `Id` | Identificador único | — |
| `Titulo` | Título del reporte | — |
| `FechaCreacion` | Fecha de generación | — |
| `Categoria` | Categoría del reporte | — |
| `Filtro` | Filtros aplicados | — |
| `Datos` | Datos del reporte (serializados) | — |
| `GeneradoPor` | Quién lo generó | — |

### `WSMessageAPIData`

Credenciales/datos de integración con la API externa de mensajería (WhatsApp/WebSocket).

| Columna | Descripción | Relación |
|---|---|---|
| `ClientId` | Identificador del cliente de la API | — |
| `ApiKey` | Clave de acceso | — |
| `Message` | Mensaje de estado/configuración | — |
| `Update` | Última actualización | — |

### `integration_credentials`

Credenciales de integraciones externas con control de límite de uso diario.

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `provider` | Proveedor de la integración | — |
| `secret_value` | Valor secreto/credencial | — |
| `label` | Etiqueta descriptiva | — |
| `daily_limit` | Límite diario de uso | — |
| `usage_count` | Uso acumulado del día | — |
| `usage_date` | Fecha del contador de uso | — |
| `is_active` | Si está activa | — |
| `creado` / `actualizado` | Fechas de auditoría | — |

### `worker_configs`

Configuración de los Workers (tareas en background) del sistema.

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `worker_name` | Nombre del worker | — |
| `is_enabled` | Si está habilitado | — |
| `interval_value` / `interval_unit` | Frecuencia de ejecución | — |
| `retention_value` / `retention_unit` | Política de retención de datos | — |
| `generate_enabled` | Si genera datos/reportes automáticamente | — |
| `last_updated` | Última modificación | — |

### `worker_process_events`

Registro de eventos/errores producidos por la ejecución de los Workers.

| Columna | Descripción | Relación |
|---|---|---|
| `id` | Identificador único | — |
| `name_event` | Nombre del evento | — |
| `type_event` | Tipo de evento | — |
| `title` | Título descriptivo | — |
| `error_message` | Mensaje de error (si aplica) | — |
| `vinculation_data` | Datos de vinculación del evento | — |
| `intents` | Cantidad de intentos realizados | — |
| `is_resolved` | Si fue resuelto | — |
| `created` | Fecha de creación | — |
| `updated_at` | Última actualización | — |
| `resolved_at` | Fecha de resolución | — |
| `snapshot` | Estado capturado al momento del evento | — |

### `__EFMigrationsHistory`

Tabla técnica de Entity Framework Core para el control de migraciones aplicadas.

| Columna | Descripción | Relación |
|---|---|---|
| `MigrationId` | Identificador de la migración | — |
| `ProductVersion` | Versión de EF Core usada | — |
