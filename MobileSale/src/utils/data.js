function rowConvertToColumn(data, columns, columnName, titleName, exclude = []) {
  let rows = [];
  for (let i = 0; i < columns.length; i++) {
    const currentColumnName = columns[i];
    if (currentColumnName == columnName || exclude.includes(currentColumnName)) {
      continue;
    }

    let row = {};
    row[titleName] = currentColumnName;
    for (let j = 0; j < data.length; j++) {
      row[data[j][columnName]] = data[j][currentColumnName];
    }
    rows.push(row);
  }
  return rows;
}

export { rowConvertToColumn };
