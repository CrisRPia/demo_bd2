# Demo de Monitoreo Multi-Plataforma

Esta demostración presenta dos soluciones de monitoreo populares (Prometheus/Grafana y Zabbix) monitoreando la misma base de datos PostgreSQL, permitiendo comparar sus enfoques y capacidades.

## Arquitectura

- **Base de Datos PostgreSQL**: La aplicación objetivo siendo monitoreada
- **Simulador de Consultas**: Genera carga realista en la base de datos para demostración
- **Prometheus + Grafana**: Recolección moderna de métricas y visualización
- **Zabbix**: Monitoreo empresarial con alertas y gestión de hosts

## Requisitos Previos

- Docker y Docker Compose instalados
- Puertos 3000, 8080 y 9090 disponibles en tu sistema

## Levantar el ambiente.

1. **Clonar e Iniciar**

   ```bash
   docker-compose up --build
   ```

2. **Esperar los Servicios** (2-3 minutos para que todos los servicios se inicialicen)

3. **Importar Configuración de Zabbix**
   - Ir a http://localhost:8080
   - Usuario: `admin` / Contraseña: `zabbix`
   - Navegar a **Configuration → Hosts**
   - Hacer clic en **Import** (arriba a la derecha)
   - Subir el archivo [de host](./zabbix/zbx_export_hosts.json)  proporcionado
   - Hacer clic en **Import** con la configuración predeterminada

## Accediendo a las Plataformas de Monitoreo

### Grafana (Dashboards Modernos)

- **URL**: http://localhost:3000
- **Usuario**: `admin` / **Contraseña**: `admin` (cambiar en el primer login)
- **Características**:
  - Fuente de datos PostgreSQL preconfigurada
  - Aprovisionamiento automático de dashboards
  - Visualización moderna y alertas

### Prometheus (Recolección de Métricas)

- **URL**: http://localhost:9090
- **Características**:
  - Navegador de métricas en crudo
  - Lenguaje de consultas (PromQL)
  - Monitoreo de estado de objetivos
- **Objetivos Clave**: Verificar `/targets` para confirmar que postgres-exporter está activo

### Zabbix (Monitoreo Empresarial)

- **URL**: http://localhost:8080
- **Usuario**: `Admin` / **Contraseña**: `zabbix`
- **Características**:
  - Monitoreo integral de hosts
  - Alertas y notificaciones integradas
  - Descubrimiento de hosts y servicios

## Qué Verás

### Métricas de PostgreSQL Disponibles:

- **Estadísticas de Conexión**: Conexiones activas, límites de conexión
- **Rendimiento de Consultas**: Tiempos de ejecución, consultas lentas
- **Tamaño de Base de Datos**: Tamaños de tablas, uso de índices
- **Recursos del Sistema**: CPU, memoria, E/S de disco
- **Rendimiento de Buffer**: Ratios de acierto de caché, estadísticas de buffer

### Dashboards de Grafana

- Navegar a **Dashboards** para ver los dashboards de PostgreSQL preconfigurados
- Gráficos y visualización de métricas en tiempo real

### Monitoreo de Zabbix

Después de importar la configuración del host:

- Ir a **Monitoring → Hosts** para ver el estado del host PostgreSQL
- **Monitoring → Latest Data** muestra métricas en tiempo real
- **Monitoring → Dashboards** proporciona dashboards de resumen
- **Monitoring → Problems** muestra cualquier alerta activada

### Consultas de Prometheus

Prueba estas consultas de ejemplo en Prometheus:

```promql
# Conexiones de base de datos
pg_stat_database_numbackends

# Tasa de consultas
rate(pg_stat_database_xact_commit_total[5m])

# Ratio de acierto de caché
pg_stat_database_blks_hit / (pg_stat_database_blks_read + pg_stat_database_blks_hit)
```

## Escenarios de Demostración

1. **Comparar Visualización**: Ver las mismas métricas en las tres plataformas
2. **Rendimiento de Consultas**: Ejecutar consultas en la base de datos y observar el impacto en tiempo real
3. **Configuración de Alertas**: Configurar alertas en cada plataforma para comparación
4. **Datos Históricos**: Dejar la demo corriendo para construir métricas históricas

## Detener la Demo

```bash
docker-compose down
```

Para limpiar completamente (eliminar todos los datos):

```bash
docker-compose down -v
```
