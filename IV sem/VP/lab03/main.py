class Employee:
    def __init__(self, name, salary):
        self.name = name
        self.salary = salary
    
    def work(self):
        print(f"Я работаю в компании.")
    
    def __str__(self):
        return f"Работник: {self.name}, Зарплата: {self.salary}"
    
class Manager(Employee):
    def __init__(self, name, salary, experience):
        super().__init__(name, salary)
        self.experience = experience
    
    def work(self):
        super().work()
        print("Я менеджер.")
    
    def premia(self):
        if self.experience >= 5:
            self.salary += 1000
        return self.salary
    
    def __str__(self):
        return f"Менеджер: {self.name}, Зарплата: {self.salary}, Опыт работы: {self.experience} лет"
    
class Developer(Employee):
    def __init__(self, name, salary, experience):
        super().__init__(name, salary)
        self.experience = experience
    
    def work(self):
        super().work()
        print("Я разработчик.")
    
    def premia(self):
        if self.experience >= 3:
            self.salary += 800
        return self.salary
    
    def __str__(self):
        return f"Разработчик: {self.name}, Зарплата: {self.salary}, Опыт работы: {self.experience} лет"
    
class Intern(Employee):
    def __init__(self, name, salary, experience):
        super().__init__(name, salary)
        self.experience = experience

    def work(self):
        super().work()
        print("Я стажер.")
    
    def premia(self):
        if self.experience >= 2:
            self.salary += 500
        return self.salary
    
    def __str__(self):
        return f"Стажёр: {self.name}, Зарплата: {self.salary}, Опыт работы: {self.experience} лет"

employee1 = Manager("Алиса", 5000, 6)
employee2 = Developer("Гена", 4000, 4)
employee3 = Intern("Витя", 2000, 1)

print(employee1)
print(employee2)
print(employee3)

employee1.premia()
employee2.premia()
employee3.premia()

print("\nПосле премирования:")
print(employee1)
print(employee2)
print(employee3)

total_salary = employee1.salary + employee2.salary + employee3.salary
print(f"\nОбщая зарплата: {total_salary}")

highest_salary_employee = max([employee1, employee2, employee3], key=lambda emp: emp.salary)
print(f"\nСотрудник с самой высокой зарплатой: {highest_salary_employee.name} с зарплатой {highest_salary_employee.salary}")
