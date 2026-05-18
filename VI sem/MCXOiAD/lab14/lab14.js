// 1. Создание базы данных
use lab14

// 2. Создание коллекций и добавление данных
db.services.insertMany([
  { _id: 1, name: "Консультация", price: 1000, category: "IT", duration: 60, tags: ["support", "online"] },
  { _id: 2, name: "Разработка сайта", price: 50000, category: "IT", duration: 120, tags: ["dev", "fullstack"] },
  { _id: 3, name: "Клининг", price: 3000, category: "cleaning", duration: 90, tags: ["office", "deep"] },
  { _id: 4, name: "Юрист", price: 2500, category: "legal", duration: 45, tags: ["consult", "documents"] }
])

db.orders.insertMany([
  { _id: 101, client: "ООО Ромашка", service_id: 1, quantity: 2, total: 2000, status: "completed" },
  { _id: 102, client: "ИП Иванов", service_id: 2, quantity: 1, total: 50000, status: "pending" },
  { _id: 103, client: "ООО Ромашка", service_id: 3, quantity: 3, total: 9000, status: "completed" },
  { _id: 104, client: "ЗАО Бета", service_id: 4, quantity: 5, total: 12500, status: "cancelled" }
])

// Изменение и обновление элементов
db.services.updateOne({ name: "Консультация" }, { $set: { price: 1200 } })
db.services.updateOne({ name: "Клининг" }, { $addToSet: { tags: "express" } })
db.orders.updateMany({ client: "ООО Ромашка" }, { $set: { status: "completed" } })

// 3. Выборка с условными операторами, $exists, $type, $regex, массивами
db.services.find({ price: { $gt: 2000 }, category: "IT" })
db.services.find({ duration: { $exists: true } })
db.orders.find({ total: { $type: "double" } })
db.orders.find({ client: { $regex: /^ООО/ } })
db.services.find({ tags: "online" })

// 4. Проекции
db.services.find({}, { name: 1, price: 1, _id: 0 })
db.orders.find({}, { client: 1, status: 1, total: 0 })

// 5. count()
db.orders.countDocuments()
db.orders.countDocuments({ status: "completed" })

// 6. limit() и skip()
db.orders.find().limit(2)
db.orders.find().skip(1).limit(2)

// 7. distinct()
db.orders.distinct("client")
db.services.distinct("category")

// 8. aggregate() - пустой и непустой фильтр, группировка по нескольким ключам
// Пустой фильтр
db.orders.aggregate([
  { $match: {} },
  { $group: { _id: { status: "$status", client: "$client" }, total_sum: { $sum: "$total" } } }
])

// Непустой фильтр
db.orders.aggregate([
  { $match: { status: "completed" } },
  { $group: { _id: { client: "$client", status: "$status" }, total_sum: { $sum: "$total" } } }
])