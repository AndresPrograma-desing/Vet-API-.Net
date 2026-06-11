CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;
DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260401212149_AddMetodoPago') THEN

    ALTER DATABASE CHARACTER SET utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260401212149_AddMetodoPago') THEN

    CREATE TABLE `metodos_pago` (
        `id` int(11) NOT NULL AUTO_INCREMENT,
        `nombre` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `creado` timestamp NOT NULL DEFAULT current_timestamp(),
        `actualizado` timestamp NOT NULL DEFAULT current_timestamp(),
        CONSTRAINT `PRIMARY` PRIMARY KEY (`id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260401212149_AddMetodoPago') THEN

    ALTER TABLE `citas` ADD `metodo_pago_id` int(11) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260401212149_AddMetodoPago') THEN

    CREATE INDEX `metodo_pago_id` ON `citas` (`metodo_pago_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260401212149_AddMetodoPago') THEN

    ALTER TABLE `citas` ADD CONSTRAINT `citas_ibfk_4` FOREIGN KEY (`metodo_pago_id`) REFERENCES `metodos_pago` (`id`) ON DELETE SET NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260401212149_AddMetodoPago') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260401212149_AddMetodoPago', '9.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260401230000_SeedMetodoPago') THEN

    INSERT INTO metodos_pago (nombre, creado, actualizado) SELECT 'no definido', current_timestamp(), current_timestamp() FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM metodos_pago WHERE nombre = 'no definido');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260401230000_SeedMetodoPago') THEN

    INSERT INTO metodos_pago (nombre, creado, actualizado) SELECT 'transferencia', current_timestamp(), current_timestamp() FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM metodos_pago WHERE nombre = 'transferencia');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260401230000_SeedMetodoPago') THEN

    INSERT INTO metodos_pago (nombre, creado, actualizado) SELECT 'pago movil', current_timestamp(), current_timestamp() FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM metodos_pago WHERE nombre = 'pago movil');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260401230000_SeedMetodoPago') THEN

    INSERT INTO metodos_pago (nombre, creado, actualizado) SELECT 'efectivo', current_timestamp(), current_timestamp() FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM metodos_pago WHERE nombre = 'efectivo');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260401230000_SeedMetodoPago') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260401230000_SeedMetodoPago', '9.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260408132328_AddUrlDocxToFacturas') THEN

    ALTER TABLE `facturas` ADD `url_docx` varchar(512) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260408132328_AddUrlDocxToFacturas') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260408132328_AddUrlDocxToFacturas', '9.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260410002235_FixMensajeDeleteBehavior') THEN

    ALTER TABLE `mensajes` DROP FOREIGN KEY `mensajes_ibfk_1`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260410002235_FixMensajeDeleteBehavior') THEN

    ALTER TABLE `mensajes` DROP FOREIGN KEY `mensajes_ibfk_2`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260410002235_FixMensajeDeleteBehavior') THEN

    ALTER TABLE `mensajes` ADD CONSTRAINT `mensajes_ibfk_1` FOREIGN KEY (`emisor_id`) REFERENCES `usuarios` (`id`) ON DELETE RESTRICT;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260410002235_FixMensajeDeleteBehavior') THEN

    ALTER TABLE `mensajes` ADD CONSTRAINT `mensajes_ibfk_2` FOREIGN KEY (`receptor_id`) REFERENCES `usuarios` (`id`) ON DELETE RESTRICT;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260410002235_FixMensajeDeleteBehavior') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260410002235_FixMensajeDeleteBehavior', '9.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

