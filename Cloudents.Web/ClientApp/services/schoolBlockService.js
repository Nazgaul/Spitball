

function ChipItem(objInit) {
    this.text = objInit.text;
    this.isSelected = objInit.isSelected;
};

function createChipItem(objInit) {
    return new ChipItem(objInit);
}

export default {
    createChipItem: createChipItem
}