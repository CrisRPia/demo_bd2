create table users
(
    name  text not null,
    birth date not null,
    id    serial primary key
);

create table actions (
    descriptor text not null,
    userid bigint not null references users(id),
    done_at timestamptz not null
);

-- Zabbix Monitoring User Setup

CREATE USER zbx_monitor WITH PASSWORD 'zbx_pass' INHERIT;
GRANT pg_monitor TO zbx_monitor;
