-- ADMIN: todos los usuarios con rol admin
INSERT INTO user_permissions (user_id, permissions, created_at, updated_at)
SELECT
    u.id,
    '["users.view","products.view","products.manage","messages.view","invoices.view","notifications.create","clients.view","emails.manage","settings.manage"]'::jsonb,
    now(),
    now()
FROM usuarios u
WHERE lower(u.rol) = 'admin'
ON CONFLICT (user_id) DO UPDATE SET
    permissions = EXCLUDED.permissions,
    updated_at = now();

-- SECRETARIA: todos los usuarios con rol secretaria
INSERT INTO user_permissions (user_id, permissions, created_at, updated_at)
SELECT
    u.id,
    '["citas.list","citas.create","clients.create","vaccines.view","calendar.view","invoices.manage","messages.view","invoices.view","recipes.manage","notifications.create","clients.view"]'::jsonb,
    now(),
    now()
FROM usuarios u
WHERE lower(u.rol) = 'secretaria'
ON CONFLICT (user_id) DO UPDATE SET
    permissions = EXCLUDED.permissions,
    updated_at = now();

-- DOCTOR: todos los usuarios con rol doctor
INSERT INTO user_permissions (user_id, permissions, created_at, updated_at)
SELECT
    u.id,
    '["citas.mine","vaccines.view","calendar.view","records.view","recipes.manage","messages.view","invoices.view","notifications.create"]'::jsonb,
    now(),
    now()
FROM usuarios u
WHERE lower(u.rol) = 'doctor'
ON CONFLICT (user_id) DO UPDATE SET
    permissions = EXCLUDED.permissions,
    updated_at = now();
