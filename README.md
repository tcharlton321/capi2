To use this app run the commands commented in the top of the docker file.
In postman make a login request and then use the bearer token it produces as your auth for hitting all other endpoints.
TODO: add endpoints to readme


Database ddl
```
-- DROP SCHEMA dbo;

CREATE SCHEMA dbo;
-- db1.dbo.guest definition

-- Drop table

-- DROP TABLE db1.dbo.guest;

CREATE TABLE db1.dbo.guest (
	id int IDENTITY(1,1) NOT NULL,
	title varchar(15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	name_first varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	name_last varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	suffix varchar(15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	dob date NULL,
	email varchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	addr_street varchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	addr_city varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	addr_state varchar(25) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	addr_zip varchar(25) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	created datetime DEFAULT getdate() NULL,
	RowVersion timestamp NOT NULL,
	CONSTRAINT PK_guestId PRIMARY KEY (id)
);


-- db1.dbo.stay definition

-- Drop table

-- DROP TABLE db1.dbo.stay;

CREATE TABLE db1.dbo.stay (
	id int IDENTITY(1,1) NOT NULL,
	created datetime DEFAULT getdate() NULL,
	checkin datetime NULL,
	checkout datetime NULL,
	CONSTRAINT PK_stay PRIMARY KEY (id)
);


-- db1.dbo.users definition

-- Drop table

-- DROP TABLE db1.dbo.users;

CREATE TABLE db1.dbo.users (
	id int IDENTITY(1,1) NOT NULL,
	username varchar(25) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	pass varbinary(MAX) NULL,
	created datetime DEFAULT getdate() NULL,
	CONSTRAINT PK_user PRIMARY KEY (id)
);
 CREATE  UNIQUE NONCLUSTERED INDEX Index_user_1 ON dbo.users (  username ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- db1.dbo.user_logins definition

-- Drop table

-- DROP TABLE db1.dbo.user_logins;

CREATE TABLE db1.dbo.user_logins (
	id int IDENTITY(1,1) NOT NULL,
	user_id int NULL,
	created datetime DEFAULT getdate() NULL,
	CONSTRAINT PK_user_logins PRIMARY KEY (id),
	CONSTRAINT FK_user_logins_user FOREIGN KEY (user_id) REFERENCES db1.dbo.users(id)
);
```

![master - db1 - dbo](https://github.com/tcharlton321/capi2/assets/54556932/88288150-3873-4f55-80e4-aa19d9181b35)
