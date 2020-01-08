const calculateItemsToShow = function(containerElm, slideWidth, offset, maxItemsToShow){
    let containerWidth = containerElm.offsetWidth;
    let itemCount = (containerWidth / slideWidth);
    let calculatedOffset = itemCount * offset / 100;
    calculatedOffset = itemCount - calculatedOffset > maxItemsToShow ? 0 : calculatedOffset;
    itemCount = calculatedOffset === 0 ? maxItemsToShow : itemCount;
    let itemsToShow = itemCount - calculatedOffset;
    return itemsToShow;
}

export default {
    calculateItemsToShow
}