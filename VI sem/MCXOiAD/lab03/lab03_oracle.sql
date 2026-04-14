BEGIN
   FOR obj IN (SELECT object_name, object_type FROM user_objects 
               WHERE object_name IN ('GETSUBTREE','ADDCHILDNODE','MOVESUBTREE',
                                     'SEQ_CAT','VW_CATEGORY_TREE'))
   LOOP
      IF obj.object_type = 'PROCEDURE' THEN
         EXECUTE IMMEDIATE 'DROP PROCEDURE ' || obj.object_name;
      ELSIF obj.object_type = 'SEQUENCE' THEN
         EXECUTE IMMEDIATE 'DROP SEQUENCE ' || obj.object_name;
      ELSIF obj.object_type = 'VIEW' THEN
         EXECUTE IMMEDIATE 'DROP VIEW ' || obj.object_name;
      END IF;
   END LOOP;
   
   BEGIN EXECUTE IMMEDIATE 'ALTER TABLE ServiceCategories DROP COLUMN ParentID'; 
   EXCEPTION WHEN OTHERS THEN NULL; END;
END;
/

-- 1
ALTER TABLE ServiceCategories ADD ParentID NUMBER;
ALTER TABLE ServiceCategories ADD CONSTRAINT fk_cat_parent 
   FOREIGN KEY (ParentID) REFERENCES ServiceCategories(CategoryID);
/
CREATE SEQUENCE seq_cat START WITH 1 INCREMENT BY 1;
/
-- 2
CREATE OR REPLACE PROCEDURE GetSubtree(p_node_value IN VARCHAR2) IS
   v_id NUMBER;
BEGIN
   SELECT CategoryID INTO v_id FROM ServiceCategories WHERE CategoryName = p_node_value;
   
   DBMS_OUTPUT.PUT_LINE(CHR(10) || '=== Подчиненные узлы для "' || p_node_value || '" ===');
   DBMS_OUTPUT.PUT_LINE('Уровень' || CHR(9) || 'Название' || CHR(9) || 'Путь');
   DBMS_OUTPUT.PUT_LINE('-------');
   
   FOR rec IN (
      SELECT CategoryName, LEVEL,
             SYS_CONNECT_BY_PATH(CategoryName, '/') AS FullPath
      FROM ServiceCategories
      START WITH CategoryID = v_id
      CONNECT BY PRIOR CategoryID = ParentID
      ORDER BY LEVEL, CategoryName
   ) LOOP
      DBMS_OUTPUT.PUT_LINE(rec.LEVEL || CHR(9) || rec.CategoryName || CHR(9) || rec.FullPath);
   END LOOP;
END GetSubtree;
/
-- 3
CREATE OR REPLACE PROCEDURE AddChildNode(
   p_parent_value IN VARCHAR2,
   p_child_name IN VARCHAR2,
   p_description IN VARCHAR2 DEFAULT NULL
) IS
   v_parent_id NUMBER;
   v_child_id NUMBER;
BEGIN
   SELECT CategoryID INTO v_parent_id FROM ServiceCategories WHERE CategoryName = p_parent_value;
   v_child_id := seq_cat.NEXTVAL;
   INSERT INTO ServiceCategories (CategoryID, CategoryName, Description, ParentID)
   VALUES (v_child_id, p_child_name, p_description, v_parent_id);
   DBMS_OUTPUT.PUT_LINE('Узел "' || p_child_name || '" добавлен в "' || p_parent_value || '"');
   COMMIT;
EXCEPTION
   WHEN NO_DATA_FOUND THEN
      DBMS_OUTPUT.PUT_LINE('Ошибка: Родитель "' || p_parent_value || '" не найден');
   WHEN OTHERS THEN
      DBMS_OUTPUT.PUT_LINE('Ошибка: ' || SQLERRM);
      ROLLBACK;
END AddChildNode;
/
-- 4
CREATE OR REPLACE PROCEDURE MoveSubtree(
   p_source_value IN VARCHAR2,
   p_target_value IN VARCHAR2
) IS
   v_source_id NUMBER;
   v_target_id NUMBER;
   v_is_descendant NUMBER;
BEGIN
   SELECT CategoryID INTO v_source_id FROM ServiceCategories WHERE CategoryName = p_source_value;
   SELECT CategoryID INTO v_target_id FROM ServiceCategories WHERE CategoryName = p_target_value;
   
   IF v_source_id = v_target_id THEN
      DBMS_OUTPUT.PUT_LINE('Ошибка: Нельзя переместить узел в самого себя');
      RETURN;
   END IF;
   
   BEGIN
      SELECT COUNT(*) INTO v_is_descendant
      FROM ServiceCategories
      WHERE CategoryID = v_target_id
      CONNECT BY PRIOR CategoryID = ParentID
      START WITH CategoryID = v_source_id;
   EXCEPTION
      WHEN OTHERS THEN v_is_descendant := 0;
   END;
   
   IF v_is_descendant > 0 THEN
      DBMS_OUTPUT.PUT_LINE('Ошибка: Нельзя переместить узел в своего потомка');
      RETURN;
   END IF;
   
   SAVEPOINT before_move;
   UPDATE ServiceCategories SET ParentID = v_target_id WHERE CategoryID = v_source_id;
   DBMS_OUTPUT.PUT_LINE('Ветка "' || p_source_value || '" перемещена в "' || p_target_value || '"');
   COMMIT;
EXCEPTION
   WHEN NO_DATA_FOUND THEN
      DBMS_OUTPUT.PUT_LINE('Ошибка: Категория не найдена');
   WHEN OTHERS THEN
      DBMS_OUTPUT.PUT_LINE('Ошибка: ' || SQLERRM);
      ROLLBACK TO before_move;
END MoveSubtree;
/



-- test
DELETE FROM ServiceCategories;
DROP SEQUENCE seq_cat;
CREATE SEQUENCE seq_cat START WITH 1 INCREMENT BY 1;
/
INSERT INTO ServiceCategories (CategoryID, CategoryName, ParentID) VALUES (seq_cat.NEXTVAL, 'Услуги', NULL);
INSERT INTO ServiceCategories (CategoryID, CategoryName, ParentID) VALUES (seq_cat.NEXTVAL, 'Товары', NULL);
INSERT INTO ServiceCategories (CategoryID, CategoryName, ParentID) 
   VALUES (seq_cat.NEXTVAL, 'Ремонт', (SELECT CategoryID FROM ServiceCategories WHERE CategoryName='Услуги'));
INSERT INTO ServiceCategories (CategoryID, CategoryName, ParentID) 
   VALUES (seq_cat.NEXTVAL, 'Диагностика', (SELECT CategoryID FROM ServiceCategories WHERE CategoryName='Услуги'));
INSERT INTO ServiceCategories (CategoryID, CategoryName, ParentID) 
   VALUES (seq_cat.NEXTVAL, 'Запчасти', (SELECT CategoryID FROM ServiceCategories WHERE CategoryName='Ремонт'));
INSERT INTO ServiceCategories (CategoryID, CategoryName, ParentID) 
   VALUES (seq_cat.NEXTVAL, 'Софт', (SELECT CategoryID FROM ServiceCategories WHERE CategoryName='Товары'));
COMMIT;
/
SET SERVEROUTPUT ON SIZE UNLIMITED;
/
-- 2
BEGIN GetSubtree('Услуги'); END;
/

-- 3
BEGIN AddChildNode('Ремонт', 'Тест', 'Описание'); END;
/

-- 4
BEGIN MoveSubtree('Запчасти', 'Товары'); END;
/
SELECT LPAD(' ', 2*(LEVEL-1)) || CategoryName AS Category_Tree
FROM ServiceCategories
START WITH ParentID IS NULL
CONNECT BY PRIOR CategoryID = ParentID
ORDER SIBLINGS BY CategoryName;
/