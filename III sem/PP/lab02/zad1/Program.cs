using System;
using System.Collections.Generic;

interface IStaff
{
    List<JobVacancy> getJobVacancies();
    List<Employee> getEmployees();
    List<JobTitle> getJobTitles();
    int addJobTitle(JobTitle jobTitle);
    string printJobVacancies();
    bool delJobTitle(int id);
    int openJobVacancy(JobVacancy jobVacancy);
    bool closeJobVacancy(int id);
    Employee recruit(JobVacancy jobVacancy, Person person);
    bool dismiss(int id, string reason);
}

public class Organization
{
    public int id { get; private set; }
    public string name { get; protected set; }
    public string shortName { get; protected set; }
    public string address { get; protected set; }
    public DateTime timeStamp { get; protected set; }

    public Organization() { }
    public Organization(Organization organization) { }
    public Organization(string name, string shortName, string address)
    { 
        this.name = name;
        this.shortName = shortName;
        this.address = address;
    }

    public void printInfo() { }
    public override string ToString()
    {
        return $"{name} ({shortName}), Address: {address}";
    }
}

public class University
{
    protected List<Faculty> faculties { get; set; }
    public int id { get; private set; }
    public string name { get; protected set; }
    public string shortName { get; protected set; }
    public string address { get; protected set; }

    public University() { }
    public University(University university) { }
    public University(string name, string shortName, string address)
    { 
        this.name = name;
        this.shortName = shortName;
        this.address = address;
    }

    public int addFaculty(Faculty faculty) { return 0; }
    public bool delFaculty(int id) { return false; }
    public bool updFaculty(Faculty faculty) { return false; }
    private bool verFaculty(int id) { return false; }
    public List<Faculty> getFaculties() { return new List<Faculty>(); }
    public void printInfo() { }
    public List<JobVacancy> getJobVacancies() { return new List<JobVacancy>(); }
    public int addJobTitle(JobTitle jobTitle) { return 0; }
    public bool delJobTitle(int id) { return false; }
    public int openJobVacancy(JobVacancy jobVacancy) { return 0; }
    public bool closeJobVacancy(int id) { return false; }
    public Employee recruit(JobVacancy jobVacancy, Person person) { return new Employee(); }
    public void dismiss(int id, string reason) { }

    public override string ToString()
    {
        return $"{name} ({shortName}), Address: {address}";
    }
}

public class Faculty
{
    protected List<Department> departments { get; set; }
    public int id { get; private set; }
    public string name { get; protected set; }
    public string shortName { get; protected set; }
    public string address { get; protected set; }

    public Faculty() { }
    public Faculty(Faculty faculty) { }
    public Faculty(string name, string shortName, string address)
    {
        this.name = name;
        this.shortName = shortName;
        this.address = address;
    }

    public int addDepartment(Department department) { return 0; }
    public bool delDepartment(int id) { return false; }
    public bool updDepartment(Department department) { return false; }
    private bool verDepartment(int id) { return false; }
    public List<Department> getDepartments() { return new List<Department>(); }
    public void printInfo() { }
    public List<JobVacancy> getJobVacancies() { return new List<JobVacancy>(); }
    public int addJobTitle(JobTitle jobTitle) { return 0; }
    public bool delJobTitle(int id) { return false; }
    public int openJobVacancy(JobVacancy jobVacancy) { return 0; }
    public bool closeJobVacancy(int id) { return false; }
    public Employee recruit(JobVacancy jobVacancy, Person person) { return new Employee(); }
    public void dismiss(int id, string reason) { }
    public override string ToString()
    {
        return $"{name} ({shortName}), Address: {address}";
    }
}

public class Department { }
public class JobVacancy { }
public class JobTitle { }
public class Employee { }
public class Person { }

public class Program
{
    static void Main(string[] args)
    {
        Organization organization = new Organization("My Organization", "Org", "St. Tutu");
        University myUniversity = new University("My University", "MU", "123 University St");
        Faculty scienceFaculty = new Faculty("Faculty of Science", "SCI", "Building A");
        myUniversity.addFaculty(scienceFaculty); Console.WriteLine($"{myUniversity},\n{scienceFaculty}, \n{organization}") ;

        Department physicsDepartment = new Department();
        scienceFaculty.addDepartment(physicsDepartment);

        JobVacancy professorVacancy = new JobVacancy();
        myUniversity.openJobVacancy(professorVacancy);

        Person applicant = new Person();
        Employee newProfessor = myUniversity.recruit(professorVacancy, applicant);
    }
}