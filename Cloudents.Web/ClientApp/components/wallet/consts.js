import store from '../../store/index';

export const cashOutCards =  [
    {
        cost: 1000,
        pointsForDollar: 100,
        image: store.getters['isFrymo'] ? "frymo" : "buyme1",
    }
];