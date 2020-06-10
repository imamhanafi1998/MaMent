/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2008                    */
/* Created on:     1/22/2020 10:02:30 AM                        */
/*==============================================================*/


if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('CONTENTROLE') and o.name = 'FK_CONTENTR_CONTENTRO_ROLETABL')
alter table CONTENTROLE
   drop constraint FK_CONTENTR_CONTENTRO_ROLETABL
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('CONTENTROLE') and o.name = 'FK_CONTENTR_CONTENTRO_CONTENTT')
alter table CONTENTROLE
   drop constraint FK_CONTENTR_CONTENTRO_CONTENTT
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('CONTENTTABLE') and o.name = 'FK_CONTENTT_CONTENTCO_CONTENTT')
alter table CONTENTTABLE
   drop constraint FK_CONTENTT_CONTENTCO_CONTENTT
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('CONTENTTABLE') and o.name = 'FK_CONTENTT_CONTENTST_STATUSTA')
alter table CONTENTTABLE
   drop constraint FK_CONTENTT_CONTENTST_STATUSTA
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('CONTENTTABLE') and o.name = 'FK_CONTENTT_CONTENTUS_USERTABL')
alter table CONTENTTABLE
   drop constraint FK_CONTENTT_CONTENTUS_USERTABL
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('CONTENTTYPETABLE') and o.name = 'FK_CONTENTT_CONTENTTY_STATUSTA')
alter table CONTENTTYPETABLE
   drop constraint FK_CONTENTT_CONTENTTY_STATUSTA
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ROLETABLE') and o.name = 'FK_ROLETABL_ROLESTATU_STATUSTA')
alter table ROLETABLE
   drop constraint FK_ROLETABL_ROLESTATU_STATUSTA
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('USERROLE') and o.name = 'FK_USERROLE_USERROLE_USERTABL')
alter table USERROLE
   drop constraint FK_USERROLE_USERROLE_USERTABL
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('USERROLE') and o.name = 'FK_USERROLE_USERROLE2_ROLETABL')
alter table USERROLE
   drop constraint FK_USERROLE_USERROLE2_ROLETABL
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('USERTABLE') and o.name = 'FK_USERTABL_USERSTATU_STATUSTA')
alter table USERTABLE
   drop constraint FK_USERTABL_USERSTATU_STATUSTA
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('CONTENTROLE')
            and   name  = 'CONTENTROLE2_FK'
            and   indid > 0
            and   indid < 255)
   drop index CONTENTROLE.CONTENTROLE2_FK
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('CONTENTROLE')
            and   name  = 'CONTENTROLE_FK'
            and   indid > 0
            and   indid < 255)
   drop index CONTENTROLE.CONTENTROLE_FK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('CONTENTROLE')
            and   type = 'U')
   drop table CONTENTROLE
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('CONTENTTABLE')
            and   name  = 'CONTENTSTATUS_FK'
            and   indid > 0
            and   indid < 255)
   drop index CONTENTTABLE.CONTENTSTATUS_FK
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('CONTENTTABLE')
            and   name  = 'CONTENTCONTENTTYPE_FK'
            and   indid > 0
            and   indid < 255)
   drop index CONTENTTABLE.CONTENTCONTENTTYPE_FK
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('CONTENTTABLE')
            and   name  = 'CONTENTUSER_FK'
            and   indid > 0
            and   indid < 255)
   drop index CONTENTTABLE.CONTENTUSER_FK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('CONTENTTABLE')
            and   type = 'U')
   drop table CONTENTTABLE
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('CONTENTTYPETABLE')
            and   name  = 'CONTENTTYPESTATUS_FK'
            and   indid > 0
            and   indid < 255)
   drop index CONTENTTYPETABLE.CONTENTTYPESTATUS_FK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('CONTENTTYPETABLE')
            and   type = 'U')
   drop table CONTENTTYPETABLE
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('ROLETABLE')
            and   name  = 'ROLESTATUS_FK'
            and   indid > 0
            and   indid < 255)
   drop index ROLETABLE.ROLESTATUS_FK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ROLETABLE')
            and   type = 'U')
   drop table ROLETABLE
go

if exists (select 1
            from  sysobjects
           where  id = object_id('STATUSTABLE')
            and   type = 'U')
   drop table STATUSTABLE
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('USERROLE')
            and   name  = 'USERROLE2_FK'
            and   indid > 0
            and   indid < 255)
   drop index USERROLE.USERROLE2_FK
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('USERROLE')
            and   name  = 'USERROLE_FK'
            and   indid > 0
            and   indid < 255)
   drop index USERROLE.USERROLE_FK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('USERROLE')
            and   type = 'U')
   drop table USERROLE
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('USERTABLE')
            and   name  = 'USERSTATUS_FK'
            and   indid > 0
            and   indid < 255)
   drop index USERTABLE.USERSTATUS_FK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('USERTABLE')
            and   type = 'U')
   drop table USERTABLE
go

/*==============================================================*/
/* Table: CONTENTROLE                                           */
/*==============================================================*/
create table CONTENTROLE (
   CONTENTROLEID        int                  not null,
   ROLEID               int                  not null,
   CONTENTID            int                  not null,
   constraint PK_CONTENTROLE primary key (CONTENTROLEID)
)
go

/*==============================================================*/
/* Index: CONTENTROLE_FK                                        */
/*==============================================================*/
create index CONTENTROLE_FK on CONTENTROLE (
ROLEID ASC
)
go

/*==============================================================*/
/* Index: CONTENTROLE2_FK                                       */
/*==============================================================*/
create index CONTENTROLE2_FK on CONTENTROLE (
CONTENTID ASC
)
go

/*==============================================================*/
/* Table: CONTENTTABLE                                          */
/*==============================================================*/
create table CONTENTTABLE (
   CONTENTID            int                  not null,
   USERID               int                  not null,
   STATUSID             int                  not null,
   CONTENTTYPEID        int                  not null,
   CONTENTTITLE         varchar(100)         not null,
   CONTENTDESCRIPTION   text                 not null,
   CONTENTLINK          varchar(200)         null,
   CONTENTFILEPATH      varchar(1000)        null,
   CONTENTDATE          datetime             not null,
   CONTENTKEYWORD       varchar(100)         null,
   constraint PK_CONTENTTABLE primary key (CONTENTID)
)
go

/*==============================================================*/
/* Index: CONTENTUSER_FK                                        */
/*==============================================================*/
create index CONTENTUSER_FK on CONTENTTABLE (
USERID ASC
)
go

/*==============================================================*/
/* Index: CONTENTCONTENTTYPE_FK                                 */
/*==============================================================*/
create index CONTENTCONTENTTYPE_FK on CONTENTTABLE (
CONTENTTYPEID ASC
)
go

/*==============================================================*/
/* Index: CONTENTSTATUS_FK                                      */
/*==============================================================*/
create index CONTENTSTATUS_FK on CONTENTTABLE (
STATUSID ASC
)
go

/*==============================================================*/
/* Table: CONTENTTYPETABLE                                      */
/*==============================================================*/
create table CONTENTTYPETABLE (
   CONTENTTYPEID        int                  not null,
   STATUSID             int                  not null,
   CONTENTTYPENAME      varchar(100)         not null,
   constraint PK_CONTENTTYPETABLE primary key (CONTENTTYPEID)
)
go

/*==============================================================*/
/* Index: CONTENTTYPESTATUS_FK                                  */
/*==============================================================*/
create index CONTENTTYPESTATUS_FK on CONTENTTYPETABLE (
STATUSID ASC
)
go

/*==============================================================*/
/* Table: ROLETABLE                                             */
/*==============================================================*/
create table ROLETABLE (
   ROLEID               int                  not null,
   STATUSID             int                  not null,
   ROLENAME             varchar(50)          not null,
   ROLEDESCRIPTION      text                 not null,
   constraint PK_ROLETABLE primary key (ROLEID)
)
go

/*==============================================================*/
/* Index: ROLESTATUS_FK                                         */
/*==============================================================*/
create index ROLESTATUS_FK on ROLETABLE (
STATUSID ASC
)
go

/*==============================================================*/
/* Table: STATUSTABLE                                           */
/*==============================================================*/
create table STATUSTABLE (
   STATUSID             int                  not null,
   STATUSNAME           varchar(50)          not null,
   constraint PK_STATUSTABLE primary key (STATUSID)
)
go

/*==============================================================*/
/* Table: USERROLE                                              */
/*==============================================================*/
create table USERROLE (
   USERROLEID           int                  not null,
   USERID               int                  not null,
   ROLEID               int                  not null,
   constraint PK_USERROLE primary key (USERROLEID)
)
go

/*==============================================================*/
/* Index: USERROLE_FK                                           */
/*==============================================================*/
create index USERROLE_FK on USERROLE (
USERID ASC
)
go

/*==============================================================*/
/* Index: USERROLE2_FK                                          */
/*==============================================================*/
create index USERROLE2_FK on USERROLE (
ROLEID ASC
)
go

/*==============================================================*/
/* Table: USERTABLE                                             */
/*==============================================================*/
create table USERTABLE (
   USERID               int                  not null,
   STATUSID             int                  not null,
   USERNAME             varchar(50)          not null,
   USERPASSWORD         varchar(1000)        not null,
   constraint PK_USERTABLE primary key (USERID)
)
go

/*==============================================================*/
/* Index: USERSTATUS_FK                                         */
/*==============================================================*/
create index USERSTATUS_FK on USERTABLE (
STATUSID ASC
)
go

alter table CONTENTROLE
   add constraint FK_CONTENTR_CONTENTRO_ROLETABL foreign key (ROLEID)
      references ROLETABLE (ROLEID)
go

alter table CONTENTROLE
   add constraint FK_CONTENTR_CONTENTRO_CONTENTT foreign key (CONTENTID)
      references CONTENTTABLE (CONTENTID)
go

alter table CONTENTTABLE
   add constraint FK_CONTENTT_CONTENTCO_CONTENTT foreign key (CONTENTTYPEID)
      references CONTENTTYPETABLE (CONTENTTYPEID)
go

alter table CONTENTTABLE
   add constraint FK_CONTENTT_CONTENTST_STATUSTA foreign key (STATUSID)
      references STATUSTABLE (STATUSID)
go

alter table CONTENTTABLE
   add constraint FK_CONTENTT_CONTENTUS_USERTABL foreign key (USERID)
      references USERTABLE (USERID)
go

alter table CONTENTTYPETABLE
   add constraint FK_CONTENTT_CONTENTTY_STATUSTA foreign key (STATUSID)
      references STATUSTABLE (STATUSID)
go

alter table ROLETABLE
   add constraint FK_ROLETABL_ROLESTATU_STATUSTA foreign key (STATUSID)
      references STATUSTABLE (STATUSID)
go

alter table USERROLE
   add constraint FK_USERROLE_USERROLE_USERTABL foreign key (USERID)
      references USERTABLE (USERID)
go

alter table USERROLE
   add constraint FK_USERROLE_USERROLE2_ROLETABL foreign key (ROLEID)
      references ROLETABLE (ROLEID)
go

alter table USERTABLE
   add constraint FK_USERTABL_USERSTATU_STATUSTA foreign key (STATUSID)
      references STATUSTABLE (STATUSID)
go

