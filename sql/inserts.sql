INSERT INTO departments(title, head, classroom)
VALUES ('Кафедра КБ-1 "Защита информации"', 'Пушкин Павел Юрьевич', '270а'),
       ('Кафедра КБ-2 "Прикладные информационные технологии"', 'Трубиенко Олег Владимирович', '216'),
       ('Кафедра КБ-4 "Интеллектуальные системы информационной безопасности"', 'Магомедов Шамиль Гасангусейнович',
        '234'),
       ('Кафедра КБ-5 "Аппаратное, программное и математическое обеспечение вычислительных систем"',
        'Кулагин Владимир Петрович', '323');

INSERT INTO groups(cypher, department_id)
VALUES ('БСБО-01-20', 1),
       ('ИКБО-01-18', 1),
       ('БИСО-01-20', 2),
       ('БИСО-02-20', 2),
       ('БИСО-01-19', 2),
       ('ИСБО-01-21', 3),
       ('ИКСО-01-20', 3),
       ('АПБО-01-17', 4),
       ('АПСО-01-18', 4),
       ('НПСО-01-21', 4);


INSERT INTO students(name, surname, middle_name, department_id, group_id)
VALUES ('Алексей', 'Иванов', 'Сергеевич', 1, 1),
       ('Анастасия', 'Иванова', 'Артемова', 1, 1),
       ('Сергей', 'Петров', 'Потапович', 1, 1),
       ('Екатерина', 'Петрова', 'Юрьевна', 1, 2),
       ('Георгий', 'Семенов', 'Юрьевич', 1, 2),
       ('Юлия', 'Семенова', 'Сергеевна', 1, 2),
       ('Георгий', 'Алексеев', 'Иванович', 2, 3),
       ('Полина', 'Алексеева', 'Дмитриевна', 2, 3),
       ('Михаил', 'Городов', 'Владимирович', 2, 3),
       ('Андрей', 'Смирнов', 'Егорович', 2, 3),
       ('Светлана', 'Информатова', 'Романовна', 2, 3),
       ('Павел', 'Мякишев', 'Никитич', 2, 4),
       ('Дарья', 'Крутая', 'Ильична', 2, 4),
       ('Таисия', 'Птицева', 'Павловна', 2, 4),
       ('Иван', 'Хорошев', 'Романович', 2, 4),
       ('Яков', 'Щелков', 'Сергеевич', 2, 5),
       ('Ульяна', 'Алексеева', 'Олеговна', 2, 5),
       ('Юрий', 'Гагарин', 'Матвеевич', 3, 6),
       ('Юлия', 'Неймова', 'Сергеевна', 3, 6),
       ('Николай', 'Иванов', 'Геннадьевич', 3, 7),
       ('Екатерина', 'Гудкова', 'Кирилловна', 4, 8),
       ('Артем', 'Ванифатов', 'Родионович', 4, 8),
       ('Артем', 'Иванов', 'Егорович', 4, 9);


INSERT INTO subjects(title) VALUES ('Языки программирования'), ('Психология'), ('Философия'),
                                   ('История'), ('Математический анализ'), ('Иностранный язык'),
                                   ('Физическая культура и спорт'), ('Физика'), ('Базы данных');

INSERT INTO marks(mark, passes, student_id, subject_id) VALUES
    (5, 1, 1, 1),  (5, 0, 1, 2),  (5, 1, 1, 3),  (5, 2, 1, 4), --1
    (4, 2, 2, 2),  (5, 0, 2, 4),  (5, 2, 2, 5),  (5, 3, 2, 6), --2
    (3, 3, 3, 4),  (5, 3, 3, 6),  (4, 2, 3, 7),  (5, 1, 3, 9), --3
    (5, 4, 4, 2),  (4, 2, 4, 3),  (5, 2, 4, 4),  (5, 2, 4, 5), --4
    (5, 4, 5, 3),  (5, 4, 5, 4),  (4, 2, 5, 5),  (4, 3, 5, 6), --5
    (5, 3, 6, 4),  (4, 3, 6, 6),  (3, 3, 6, 7),  (5, 4, 6, 8), --6
    (4, 2, 7, 3),  (5, 2, 7, 4),  (3, 4, 7, 8),  (4, 3, 7, 9), --7
    (4, 1, 8, 3),  (3, 3, 8, 5),  (4, 3, 8, 6),  (3, 2, 8, 7), --8
    (4, 4, 9, 2),  (2, 4, 9, 3),  (3, 2, 9, 4),  (4, 3, 9, 6), --9
    (3, 3, 10, 1), (3, 5, 10, 2), (5, 3, 10, 3), (3, 4, 10, 4), --10
    (3, 2, 11, 2), (5, 3, 11, 3), (2, 2, 11, 4), (2, 5, 11, 5), --11
    (2, 3, 12, 3), (4, 3, 12, 4), (4, 3, 12, 5), (3, 6, 12, 6), --12
    (2, 4, 13, 3), (5, 0, 13, 4), (2, 8, 13, 5), (2, 9, 13, 7), --13
    (5, 3, 14, 2), (4, 4, 14, 6), (5, 2, 14, 7), (3, 2, 14, 8), --14
    (5, 2, 15, 2), (3, 3, 15, 3), (5, 3, 15, 4), (5, 0, 15, 5), --15
    (4, 1, 16, 2), (4, 3, 16, 4), (5, 4, 16, 5), (4, 0, 16, 9), --16
    (5, 0, 17, 1), (5, 4, 17, 5), (4, 1, 17, 7), (3, 0, 17, 9), --17
    (4, 0, 18, 1), (4, 5, 18, 2), (5, 2, 18, 3), (5, 0, 18, 6), --18
    (5, 0, 19, 3), (4, 2, 19, 4), (4, 1, 19, 5), (5, 2, 19, 7), --19
    (4, 2, 20, 1), (5, 1, 20, 2), (3, 2, 20, 3), (4, 3, 20, 4), --20
    (5, 1, 21, 2), (4, 2, 21, 7), (5, 3, 21, 3), (5, 2, 21, 6), --21
    (3, 2, 22, 1), (5, 3, 22, 2), (5, 4, 22, 3), (4, 0, 22, 7), --22
    (2, 6, 23, 2), (4, 1, 23, 3), (4, 2, 23, 4), (5, 1, 23, 6); --23