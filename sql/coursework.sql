--CREATE DATABASE coursework;

CREATE TABLE departments
(
    id        SERIAL PRIMARY KEY,
    title     VARCHAR(100) NOT NULL,
    head      VARCHAR(50)  NOT NULL,
    classroom VARCHAR(6)   NOT NULL
);

CREATE TABLE "groups"
(
    id            SERIAL PRIMARY KEY,
    cypher        VARCHAR(10)   NOT NULL,
    student_count INT DEFAULT 0 NOT NULL,
    department_id INT           NOT NULL
        CONSTRAINT
            groups_departments_id_fk REFERENCES departments
            ON DELETE CASCADE
);

CREATE TABLE students
(
    id            SERIAL PRIMARY KEY,
    surname       VARCHAR(20) NOT NULL,
    "name"        VARCHAR(20) NOT NULL,
    middle_name   VARCHAR(20) NOT NULL,
    department_id INT         NOT NULL
        CONSTRAINT
            students_departments_id_fk REFERENCES departments
            ON DELETE CASCADE,
    group_id      INT         NOT NULL
        CONSTRAINT
            students_groups_id_fk REFERENCES "groups"
            ON DELETE CASCADE
);

CREATE TABLE subjects
(
    id    SERIAL PRIMARY KEY,
    title VARCHAR(30) NOT NULL
);

CREATE TABLE marks
(
    id         SERIAL PRIMARY KEY,
    mark       INT NOT NULL CHECK (mark BETWEEN 2 AND 5),
    passes     INT NOT NULL CHECK (passes >= 0),
    student_id INT NOT NULL
        CONSTRAINT
            marks_students_id_fk REFERENCES students
            ON DELETE CASCADE,
    subject_id INT NOT NULL
        CONSTRAINT
            marks_subjects_id_fk REFERENCES subjects
            ON DELETE CASCADE,
    CONSTRAINT marks_student_subject_unique UNIQUE (student_id, subject_id)
);

/* VIEW из задания 3b (перенесен в начало, чтобы скрипт исправно работал*/
CREATE VIEW marks_view (mark_id, group_name, surname, name, middle_name, subj_title, mark, passes) AS
SELECT marks.id,
       g.cypher,
       st.surname,
       st.name,
       st.middle_name,
       sj.title,
       marks.mark,
       marks.passes
FROM marks
         JOIN students st ON marks.student_id = st.id
         JOIN subjects sj ON marks.subject_id = sj.id
         JOIN "groups" g ON st.group_id = g.id
ORDER BY cypher, surname;
/* END */

/* ЗАДАНИЕ 4. Индексы */

DROP EXTENSION IF EXISTS pg_trgm;
CREATE EXTENSION pg_trgm; -- расширение для использования индекса GIN

DROP INDEX IF EXISTS trgm_idx_groups_cypher, idx_marks_mark,
    idx_marks_student_subject_id;

-- noinspection SqlResolve
CREATE INDEX trgm_idx_groups_cypher ON "groups" USING gin(cypher gin_trgm_ops);
CREATE INDEX idx_marks_mark ON marks(mark);
CREATE UNIQUE INDEX idx_marks_student_subject_id ON marks(student_id, subject_id);

SET enable_seqscan = false; -- отключение поиска полным перебором, чтобы показать работу индекса
EXPLAIN ANALYZE SELECT * FROM "groups" WHERE cypher LIKE '%-20';
EXPLAIN ANALYZE SELECT * FROM marks WHERE mark = 4;
EXPLAIN ANALYZE SELECT * FROM marks WHERE student_id > 3 AND subject_id = 2;
EXPLAIN ANALYZE SELECT * FROM students WHERE id > 10;
SET enable_seqscan = default; -- возвращаем исходное значение

/* ЗАДАНИЕ 5. Триггеры */

/* Триггер добавление студента */
CREATE OR REPLACE FUNCTION student_insert()
    RETURNS TRIGGER
AS
$$
BEGIN
    UPDATE "groups"
    SET student_count = student_count + 1
    WHERE id = new.group_id;
    RETURN new;
END
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS tr_student_new ON students;
CREATE TRIGGER tr_student_new
    AFTER INSERT
    ON students
    FOR EACH ROW
EXECUTE PROCEDURE student_insert();
/* END */

/* Триггер удаление студента */
CREATE OR REPLACE FUNCTION student_delete()
    RETURNS TRIGGER
AS
$$
BEGIN
    UPDATE "groups"
    SET student_count = student_count - 1
    WHERE id = old.group_id;
    RETURN new;
END
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS tr_student_delete ON students;
CREATE TRIGGER tr_student_delete
    AFTER DELETE
    ON students
    FOR EACH ROW
EXECUTE PROCEDURE student_delete();
/* END */

/* Триггер обновление студента */
CREATE OR REPLACE FUNCTION student_update()
    RETURNS TRIGGER
AS
$$
BEGIN
    UPDATE "groups"
    SET student_count = student_count + 1
    WHERE id = new.group_id;
    UPDATE "groups"
    SET student_count = student_count - 1
    WHERE id = old.group_id;
    RETURN new;
END
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS tr_student_update ON students;
CREATE TRIGGER tr_student_update
    AFTER UPDATE
    ON students
    FOR EACH ROW
EXECUTE PROCEDURE student_update();
/* END */


/* ЗАДАНИЕ 6. Операции добавления, удаления и изменения */

/* добавление, удаление, изменение DEPARTMENTS */
CREATE OR REPLACE FUNCTION add_department(
    p_title VARCHAR,
    p_head VARCHAR,
    p_classroom VARCHAR)
    RETURNS VARCHAR
AS
$$
BEGIN
    INSERT INTO departments(title, head, classroom)
    VALUES (p_title, p_head, p_classroom);
    return 'Кафедра успешно добавлена.';
END
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION delete_department(department_id INT)
    RETURNS VARCHAR
AS
$$
BEGIN
    if (not exists(SELECT * FROM departments WHERE id = department_id)) then
        return 'Кафедры с данным id не существует.';
    end if;
    DELETE FROM departments WHERE id = department_id;
    return 'Кафедра успешно удалена.';
END
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION update_department(
    p_id INT,
    p_title VARCHAR DEFAULT NULL,
    p_head VARCHAR DEFAULT NULL,
    p_classroom VARCHAR DEFAULT NULL)
    RETURNS VARCHAR
AS
$$
BEGIN
    if (not exists(SELECT * FROM departments WHERE id = p_id)) then
        raise exception 'Кафедры с данным id не существует.';
    end if;
    if (p_title is not null) then
        UPDATE departments
        SET title = p_title
        WHERE id = p_id;
    end if;
    if (p_head is not null) then
        UPDATE departments
        SET head = p_head
        WHERE id = p_id;
    end if;
    if (p_classroom is not null) then
        UPDATE departments
        SET classroom = p_classroom
        WHERE id = p_id;
    end if;
    return 'Кафедра успешно обновлена.';
END
$$ LANGUAGE plpgsql;
/* END */

/* добавление, удаление, изменение GROUPS */
CREATE OR REPLACE FUNCTION add_group(
    p_cypher VARCHAR,
    p_department_id INT)
    RETURNS VARCHAR
AS
$$
BEGIN
    if (not exists(SELECT * FROM departments WHERE id = p_department_id)) then
        raise exception 'Кафедры с данным id не существует.';
    end if;
    INSERT INTO "groups" (cypher, department_id)
    VALUES (p_cypher, p_department_id);
    return 'Группа успешно добавлена.';
END
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION delete_group(group_id INT)
    RETURNS VARCHAR
AS
$$
BEGIN
    if (not exists(SELECT * FROM "groups" WHERE id = group_id)) then
        return 'Группы с данным id не существует.';
    end if;
    DELETE FROM "groups" WHERE id = group_id;
    return 'Группа успешно удалена.';
END
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION update_group(
    p_id INT,
    p_cypher VARCHAR DEFAULT NULL,
    p_department_id INT DEFAULT NULL)
    RETURNS VARCHAR
AS
$$
BEGIN
    if (not exists(SELECT * FROM "groups" WHERE id = p_id)) then
        raise exception 'Группы с данным id не существует';
    end if;
    if (p_cypher is not null) then
        UPDATE "groups"
        SET cypher = p_cypher
        WHERE id = p_id;
    end if;
    if (p_department_id is not null) then
        UPDATE "groups"
        SET department_id = p_department_id
        WHERE id = p_id;
    end if;
    return 'Группа успешно обновлена.';
END
$$ LANGUAGE plpgsql;
/* END */

/* добавление, удаление, изменение STUDENTS */
CREATE OR REPLACE FUNCTION add_student(
    p_surname VARCHAR,
    p_name VARCHAR,
    p_middle_name VARCHAR,
    p_department_id INT,
    p_group_id INT)
    RETURNS VARCHAR
AS
$$
BEGIN
    if (not exists(SELECT * FROM departments WHERE id = p_department_id)) then
        raise exception 'Кафедры с данным id не существует.';
    end if;
    if (not exists(SELECT * FROM "groups" WHERE id = p_group_id)) then
        raise exception 'Группы с данным id не существует.';
    end if;
    INSERT INTO students(name, surname, middle_name, department_id, group_id)
    VALUES (p_name, p_surname, p_middle_name, p_department_id, p_group_id);
    return 'Студент успешно добавлен.';
END
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION delete_student(student_id INT)
    RETURNS VARCHAR
AS
$$
BEGIN
    if (not exists(SELECT * FROM students WHERE id = student_id)) then
        return 'Студента с данным id не существует.';
    end if;
    DELETE FROM students WHERE id = student_id;
    return 'Студент успешно удален.';
END
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION update_student(
    p_id INT,
    p_surname VARCHAR DEFAULT NULL,
    p_name VARCHAR DEFAULT NULL,
    p_middle_name VARCHAR DEFAULT NULL,
    p_department_id INT DEFAULT NULL,
    p_group_id INT DEFAULT NULL)
    RETURNS VARCHAR
AS
$$
BEGIN
    if (not exists(SELECT * FROM students WHERE id = p_id)) then
        raise exception 'Студента с данным id не существует.';
    end if;
    if (p_name is not null) then
        UPDATE students
        SET name = p_name
        WHERE id = p_id;
    end if;
    if (p_surname is not null) then
        UPDATE students
        SET surname = p_surname
        WHERE id = p_id;
    end if;
    if (p_middle_name is not null) then
        UPDATE students
        SET middle_name = p_middle_name
        WHERE id = p_id;
    end if;
    if (p_department_id is not null) then
        UPDATE students
        SET department_id = p_department_id
        WHERE id = p_id;
    end if;
    if (p_group_id is not null) then
        UPDATE students
        SET group_id = p_group_id
        WHERE id = p_id;
    end if;
    return 'Данные студента успешно обновлены.';
END
$$ LANGUAGE plpgsql;
/* END */

/* добавление, удаление, изменение MARKS */
CREATE OR REPLACE FUNCTION add_mark(
    p_mark INT,
    p_passes INT,
    p_student_id INT,
    p_subject_id INT)
    RETURNS VARCHAR
AS
$$
BEGIN
    if (exists(SELECT * FROM marks WHERE student_id = p_student_id AND subject_id = p_subject_id)) then
        raise exception 'Данная оценка уже записана.';
    end if;
    if (not exists(SELECT * FROM students WHERE id = p_student_id)) then
        raise exception 'Студента с данным id не существует.';
    end if;
    if (not exists(SELECT * FROM subjects WHERE id = p_subject_id)) then
        raise exception 'Предмета с данным id не существует.';
    end if;
    INSERT INTO marks(mark, passes, student_id, subject_id)
    VALUES (p_mark, p_passes, p_student_id, p_subject_id);
    return 'Оценка успешно добавлена.';
END
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION delete_mark(mark_id INT)
    RETURNS VARCHAR
AS
$$
BEGIN
    if (not exists(SELECT * FROM marks WHERE id = mark_id)) then
        return 'Оценки с данным id не существует.';
    end if;
    DELETE FROM marks WHERE id = mark_id;
    return 'Оценка успешно удалена.';
END;

$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION update_mark(
    p_id INT,
    p_mark INT DEFAULT NULL,
    p_passes INT DEFAULT NULL,
    p_student_id INT DEFAULT NULL,
    p_subject_id INT DEFAULT NULL)
    RETURNS VARCHAR
AS
$$
BEGIN
    if (not exists(SELECT * FROM marks WHERE id = p_id)) then
        raise exception 'Оценки с данным id не существует.';
    end if;
    if (p_mark IS NOT NULL)
    then
        UPDATE marks SET mark = p_mark WHERE id = p_id;
    end if;
    if (p_passes IS NOT NULL)
    then
        UPDATE marks SET passes = p_passes WHERE id = p_id;
    end if;
    if (p_student_id IS NOT NULL)
    then
        UPDATE marks SET student_id = p_student_id WHERE id = p_id;
    end if;
    if (p_subject_id IS NOT NULL)
    then
        UPDATE marks SET subject_id = p_subject_id WHERE id = p_id;
    end if;
    return 'Оценка успешно обновлена.';
END
$$ LANGUAGE plpgsql;
/* END */

/* добавление, удаление, изменение SUBJECTS */
CREATE OR REPLACE FUNCTION add_subject(p_title VARCHAR)
    RETURNS VARCHAR
AS
$$
BEGIN
    INSERT INTO subjects(title) VALUES (p_title);
    return 'Предмет успешно добавлен.';
END
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION delete_subject(subject_id INT)
    RETURNS VARCHAR
AS
$$
BEGIN
    if (not exists(SELECT * FROM subjects WHERE id = subject_id)) then
        return 'Предмета с данным id не существует.';
    end if;
    DELETE FROM subjects WHERE id = subject_id;
    return 'Предмет успешно удален.';
END
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION update_subject(
    p_id INT,
    p_title VARCHAR)
    RETURNS VARCHAR
AS
$$
BEGIN
    if (not exists(SELECT * FROM subjects WHERE id = p_id)) then
        raise exception 'Предмета с данным id не существует.';
    end if;
    if (p_title IS NOT NULL)
    then
        UPDATE subjects SET title = p_title WHERE id = p_id;
    end if;
    return 'Предмет успешно обновлен.';
END
$$ LANGUAGE plpgsql;
/* END */

/* изменение marks_view */
CREATE OR REPLACE FUNCTION update_marks_view(
    p_mark_id INT,
    p_group_name VARCHAR,
    p_surname VARCHAR,
    p_name VARCHAR,
    p_middle_name VARCHAR,
    p_subj_title VARCHAR,
    p_mark INT,
    p_passes INT)
    RETURNS VARCHAR AS
$$
BEGIN
    if (not exists(SELECT * FROM marks_view WHERE mark_id = p_mark_id)) then
        raise exception 'Строки с данным id не существует.';
    end if;
    if (p_group_name is not null) then
        UPDATE marks_view SET group_name = p_group_name WHERE mark_id = p_mark_id;
    end if;
    if (p_surname is not null) then
        UPDATE marks_view SET surname = p_surname WHERE mark_id = p_mark_id;
    end if;
    if (p_name is not null) then
        UPDATE marks_view SET name = p_name WHERE mark_id = p_mark_id;
    end if;
    if (p_middle_name is not null) then
        UPDATE marks_view SET middle_name = p_middle_name WHERE mark_id = p_mark_id;
    end if;
    if (p_subj_title is not null) then
        UPDATE marks_view SET subj_title = p_subj_title WHERE mark_id = p_mark_id;
    end if;
    if (p_mark is not null) then
        UPDATE marks_view SET mark = p_mark WHERE mark_id = p_mark_id;
    end if;
    if (p_passes is not null) then
        UPDATE marks_view SET passes = p_passes WHERE mark_id = p_mark_id;
    end if;
    return 'Строка успешно обновлена.';
END;
$$ LANGUAGE plpgsql;
/* END */

/* ЗАДАНИЕ 7. Реализовать отдельную хранимую процедуру или функцию, состоящую из
    нескольких отдельных операций в виде единой транзакции, которая при
    определенных условиях может быть зафиксирована или откатана; */

CREATE OR REPLACE PROCEDURE transfer_student(
    st_id INT,
    gr_id INT)
AS
$$
declare
    old_gr_id int;
BEGIN
    old_gr_id = (SELECT group_id FROM students WHERE id = st_id);
    UPDATE students
    SET group_id      = gr_id,
        department_id = (SELECT department_id FROM "groups" WHERE id = gr_id)
    WHERE id = st_id;
    if ((SELECT mark FROM marks WHERE student_id = st_id AND mark = 2) > 0)
    then
        rollback;
        raise exception 'Перевод студента невозможен при наличии пересдач';
    end if;
    if ((SELECT RIGHT(cypher, 2) FROM "groups" WHERE id = old_gr_id) !=
        (SELECT RIGHT(cypher, 2)
         FROM "groups"
         WHERE id = (SELECT group_id FROM students WHERE id = st_id)))
    then
        rollback;
        raise exception 'Невозможно перевести студента в группу с другого курса';
    end if;

    commit;
END
$$ LANGUAGE plpgsql;


/* ЗАДАНИЕ 8. Реализовать курсор на обновления отдельных
    данных (вычисления значения полей выбранной таблицы) */

CREATE OR REPLACE PROCEDURE curs_UpdateSubject(
    sj_id INT,
    sj_title VARCHAR(40))
AS
$$
DECLARE
    curs_update CURSOR FOR SELECT *
                           FROM subjects;
    id    INT;
    title VARCHAR(40);
BEGIN
    open curs_update;
    loop
        fetch curs_update into id, title;
        if not FOUND then exit; end if;
        if (sj_id = id) then
            UPDATE subjects SET title = sj_title WHERE current of curs_update;
        end if;
    end loop;
END;
$$ LANGUAGE plpgsql;


/* ЗАДАНИЕ 9. Реализовать собственную скалярную и векторную функции.
    Функции сохранить в базе данных. */

CREATE OR REPLACE FUNCTION passes_count(s_id INT)
    RETURNS INT AS
$$
DECLARE
    i            INT;
    j            INT;
    passes_count INT := 0;
BEGIN
    if not exists(SELECT * FROM students WHERE id = s_id)
    then
        raise exception 'Данный студент не существует';
    end if;
    for i in SELECT id FROM marks
        loop
            j := (SELECT passes FROM marks WHERE id = i AND student_id = s_id);
            if j is not null
            then
                passes_count = passes_count + j;
            end if;
        end loop;
    RETURN passes_count;
END
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION depart_classrooms()
    RETURNS TABLE
            (
                class VARCHAR(6)
            )
AS
$$
BEGIN
    RETURN QUERY SELECT classroom FROM departments;
END;
$$ LANGUAGE plpgsql;


/* Задание 10. Распределение прав пользователей: предусмотреть не менее
    двух пользователей с разным набором привилегий. Каждый набор привилегий
    оформить в виде роли. */

CREATE ROLE "User" WITH PASSWORD 'user' LOGIN;
GRANT SELECT ON ALL TABLES IN SCHEMA public TO "User";
GRANT SELECT ON marks_view TO "User";
CREATE ROLE "Admin" WITH PASSWORD 'admin' SUPERUSER LOGIN;


/* ЗАДАНИЕ 3. Реализовать запросы по заданиям */

/* 3a. Составной многотабличный запрос с CASE-выражением */
SELECT marks.id AS mark_id,
       s.surname,
       s.name,
       s.middle_name,
       sj.title AS subject,
       CASE
           WHEN mark = 5 THEN 'Отлично'
           WHEN mark = 4 THEN 'Хорошо'
           WHEN mark = 3 THEN 'Удовлетворительно'
           WHEN mark = 2 THEN 'Пересдача'
           END  AS mark
FROM marks
         JOIN students s ON marks.student_id = s.id
         JOIN subjects sj ON marks.subject_id = sj.id
ORDER BY surname;
/* END */

/* 3b. Многотабличный VIEW с возможностью его обновления */
/* Создание VIEW в начале скрипта! */
CREATE OR REPLACE FUNCTION marks_view_updatable()
    RETURNS TRIGGER AS
$$
BEGIN
    UPDATE students
    SET name        = new.name,
        surname     = new.surname,
        middle_name = new.middle_name
    WHERE id = (SELECT student_id FROM marks WHERE marks.id = old.mark_id);
    UPDATE "groups"
    SET cypher = new.group_name
    WHERE id = (SELECT group_id
                FROM students
                WHERE id = (SELECT student_id FROM marks WHERE marks.id = old.mark_id));
    UPDATE subjects
    SET title = new.subj_title
    WHERE id = (SELECT subject_id FROM marks WHERE marks.id = old.mark_id);
    UPDATE marks
    SET mark   = new.mark,
        passes = new.passes
    WHERE id = old.mark_id;
    RETURN new;
END
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS tr_marks_view_updatable ON marks_view;
CREATE TRIGGER tr_marks_view_updatable
    INSTEAD OF UPDATE
    ON marks_view
    FOR EACH ROW
EXECUTE PROCEDURE marks_view_updatable();
/* END */

/* 3c. Запросы, содержащие подзапрос в разделах SELECT, FROM и WHERE (в каждом хотя бы по одному); */
SELECT (SELECT surname FROM students WHERE surname = 'Иванов' LIMIT 1);

SELECT * FROM (SELECT surname, name, group_id FROM students) AS m
    WHERE m.group_id = 3;

SELECT count(mark) AS resits FROM marks
    WHERE mark = (SELECT MIN(mark) FROM marks);
/* END */

/* 3d. Коррелированные подзапросы (минимум 3 запроса). */
SELECT surname, name,
       (SELECT cypher FROM "groups" WHERE id = group_id) AS "group"
    FROM students;

SELECT * FROM "groups" g WHERE NOT
    exists(SELECT * FROM students s WHERE g.id = s.group_id)
    ORDER BY id;

SELECT id, mark, passes,
       (SELECT surname FROM students WHERE student_id = students.id),
       (SELECT title FROM subjects WHERE subject_id = subjects.id)
    FROM marks;
/* END */

/* 3e. Многотабличный запрос, содержащий группировку записей,
    агрегатные функции и параметр, используемый в разделе HAVING; */
SELECT surname, name, MIN(mark) as min_mark
FROM marks JOIN students s on marks.student_id = s.id
GROUP BY surname, name
HAVING MIN(mark) = 3;
/* END */

/* 3f. Запросы, содержащий предикат ANY(SOME) или ALL (для каждого предиката); */
SELECT * FROM marks_view WHERE group_name = ANY('{БИСО-01-20,БИСО-02-20}');

SELECT cypher, student_count
FROM "groups"
WHERE student_count > ALL (SELECT AVG(student_count) FROM "groups");
/* END */