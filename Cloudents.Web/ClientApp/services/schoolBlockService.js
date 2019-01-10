

function ChipItem(objInit) {
    this.text = objInit.text;
    this.isSelected = objInit.isSelected;
};

function createChipItem(ObjInit) {
    return new ChipItem(ObjInit)
}

export default {
    createChipItem: createChipItem
}