CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250714152136_InitialCreate') THEN
    CREATE TABLE "Cargos" (
        "Id" TEXT NOT NULL,
        "Name" TEXT NOT NULL,
        "Permissions" jsonb NOT NULL,
        CONSTRAINT "PK_Cargos" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250714152136_InitialCreate') THEN
    CREATE TABLE "Clientes" (
        "Id" TEXT NOT NULL,
        "Name" TEXT NOT NULL,
        "Email" TEXT NOT NULL,
        "Phone" TEXT NOT NULL,
        "Document" TEXT NOT NULL,
        CONSTRAINT "PK_Clientes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250714152136_InitialCreate') THEN
    CREATE TABLE "Servicos" (
        "Id" TEXT NOT NULL,
        "Name" TEXT NOT NULL,
        "Description" TEXT NOT NULL,
        "DefaultPrice" decimal(18, 2) NOT NULL,
        CONSTRAINT "PK_Servicos" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250714152136_InitialCreate') THEN
    CREATE TABLE "Status" (
        "Id" TEXT NOT NULL,
        "Name" TEXT NOT NULL,
        "Color" TEXT NOT NULL,
        "Icon" TEXT NOT NULL,
        "TriggersEmail" INTEGER NOT NULL,
        CONSTRAINT "PK_Status" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250714152136_InitialCreate') THEN
    CREATE TABLE "Usuarios" (
        "Id" TEXT NOT NULL,
        "Name" TEXT NOT NULL,
        "Email" TEXT NOT NULL,
        "PasswordHash" TEXT NOT NULL,
        "RoleId" TEXT NOT NULL,
        "IsActive" INTEGER NOT NULL,
        CONSTRAINT "PK_Usuarios" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Usuarios_Cargos_RoleId" FOREIGN KEY ("RoleId") REFERENCES "Cargos" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250714152136_InitialCreate') THEN
    CREATE TABLE "OrdensDeServico" (
        "Id" INTEGER NOT NULL,
        "ClientId" TEXT NOT NULL,
        "Equipment" TEXT NOT NULL,
        "Brand" TEXT NOT NULL,
        "Model" TEXT NOT NULL,
        "SerialNumber" TEXT NOT NULL,
        "ProblemDescription" TEXT NOT NULL,
        "TechnicalSolution" TEXT NOT NULL,
        "StatusId" TEXT NOT NULL,
        "CreatedAt" TEXT NOT NULL,
        "UpdatedAt" TEXT NOT NULL,
        "CreatedByUserId" TEXT NOT NULL,
        CONSTRAINT "PK_OrdensDeServico" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_OrdensDeServico_Clientes_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clientes" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_OrdensDeServico_Status_StatusId" FOREIGN KEY ("StatusId") REFERENCES "Status" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_OrdensDeServico_Usuarios_CreatedByUserId" FOREIGN KEY ("CreatedByUserId") REFERENCES "Usuarios" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250714152136_InitialCreate') THEN
    CREATE TABLE "LogOS" (
        "Id" TEXT NOT NULL,
        "OsId" INTEGER NOT NULL,
        "UserId" TEXT NOT NULL,
        "ChangeTimestamp" TEXT NOT NULL,
        "ChangeDescription" TEXT NOT NULL,
        "Details" jsonb NOT NULL,
        CONSTRAINT "PK_LogOS" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_LogOS_OrdensDeServico_OsId" FOREIGN KEY ("OsId") REFERENCES "OrdensDeServico" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_LogOS_Usuarios_UserId" FOREIGN KEY ("UserId") REFERENCES "Usuarios" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250714152136_InitialCreate') THEN
    CREATE INDEX "IX_LogOS_OsId" ON "LogOS" ("OsId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250714152136_InitialCreate') THEN
    CREATE INDEX "IX_LogOS_UserId" ON "LogOS" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250714152136_InitialCreate') THEN
    CREATE INDEX "IX_OrdensDeServico_ClientId" ON "OrdensDeServico" ("ClientId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250714152136_InitialCreate') THEN
    CREATE INDEX "IX_OrdensDeServico_CreatedByUserId" ON "OrdensDeServico" ("CreatedByUserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250714152136_InitialCreate') THEN
    CREATE INDEX "IX_OrdensDeServico_StatusId" ON "OrdensDeServico" ("StatusId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250714152136_InitialCreate') THEN
    CREATE INDEX "IX_Usuarios_RoleId" ON "Usuarios" ("RoleId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250714152136_InitialCreate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250714152136_InitialCreate', '9.0.7');
    END IF;
END $EF$;
COMMIT;

