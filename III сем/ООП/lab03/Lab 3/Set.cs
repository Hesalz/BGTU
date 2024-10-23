using System;
using System.Collections.Generic;
using System.Linq;

namespace lab3 {
    public class Set {
        private List<int> _elements;
        public List<int> elements {
            get { return _elements; }
            set { _elements = value; }
        }

        public class Production {
            public int Id { get; set; }
            public string OrganizationName { get; set; }

            public Production(int id, string orgName) {
                Id = id;
                OrganizationName = orgName;
            }

            public override string ToString() {
                return $"{OrganizationName} (ID: {Id})";
            }
        }

        public class Developer {
            public string FullName { get; set; }
            public int Id { get; set; }
            public string Department { get; set; }

            public Developer(string fullName, int id, string department) {
                FullName = fullName;
                Id = id;
                Department = department;
            }

            public override string ToString() {
                return $"{FullName}, ID: {Id}, Отдел: {Department}";
            }
        }

        public Production production { get; set; }
        public Developer developer { get; set; }

        public Set(List<int> elements, Production production, Developer developer) {
            this.elements = elements;
            this.production = production;
            this.developer = developer;
        }

        public int this[int index] {
            get { return elements[index]; }
            set { elements[index] = value; }
        }

        public static Set operator +(Set set, int element)
        {
            set.Add(element);
            return set;
        }

        public static Set operator +(Set set1, Set set2) {
            var union = set1.elements.Union(set2.elements).ToList();
            return new Set(union, set1.production, set1.developer);
        }

        public static Set operator -(Set set, int element)
        {
            set.Remove(element);
            return set;
        }

        public static Set operator *(Set set1, Set set2) {
            var intersection = set1.elements.Intersect(set2.elements).ToList();
            return new Set(intersection, set1.production, set1.developer);
        }

        public static explicit operator int(Set set) {
            return set.elements.Count;
        }

        public static bool operator false(Set set) {
            return set.elements.Count >= 5 && set.elements.Count <= 10;
        }

        public static bool operator true(Set set) {
            return set.elements.Count < 5 || set.elements.Count > 10;
        }

        public void Add(int element) {
            if (!elements.Contains(element)) {
                elements.Add(element);
            }
        }

        public void Remove(int element) {
            elements.Remove(element);
        }

        public override string ToString() {
            return string.Join(", ", elements);
        }





        public static bool operator >(Set set, int element)
        {
            return set.elements.Count > element;
        }

        public static bool operator <(Set set, int element)
        {
            return set.elements.Count < element;
        }
    }
}
