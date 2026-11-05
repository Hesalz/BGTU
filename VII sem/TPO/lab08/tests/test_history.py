from app.database.history import HistoryDatabase


def test_history_add_and_list_recent(tmp_path):
    db = HistoryDatabase(tmp_path / 'history.db')
    db.add('embedding', 'stego.png', 5, 3.0, 42.5, 0.1, True)
    rows = db.list_recent()
    assert len(rows) == 1
    assert rows[0]['operation'] == 'embedding'
    assert rows[0]['file_path'] == 'stego.png'
    assert rows[0]['success'] == 1


