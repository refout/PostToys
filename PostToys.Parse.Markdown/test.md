# 接口文档

## 1 新增

描述

### 1.1 接口地址[url]

> GET {{baseUrl}}/get HTTP/3.0

### 1.2 请求头[header]

```json
{
  "Accept": "application/json"
}
```

### 1.3 请求参数[param]

```json
{
  "show_env": 1
}
```

### 1.4 请求体[body]

```json
{
  "count": 1,
  "desc": "test desc"
}
```

### 1.5 路径参数[pathVar]

```json
[
]
```

### 1.4 成功响应[success]

## 2 数据库查询

描述

### 1.1 数据库链接字符串[url]

> DQL host=localhost;port=5432;database=xxx;username=xxx;password=xxx; Postgresql

<!-- > DQL host=localhost;port=3306;database=testdb;username=root;password=yourpassword; MySql -->

### 1.2 请求体[body]

```sql
select *
    from changes
--     where change_id = 1 limit 1
```

### 1.4 成功响应[success]
