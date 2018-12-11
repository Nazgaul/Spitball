export default {
    calculateRankByScore: (score) => {
        if (score <= 300) {
            return 0;
        } else if (score > 300 && score <= 700) {
            return 1;
        } else if (score > 700 && score <= 1000) {
            return 2;
        } else if (score > 1000) {
            return 3;
        }
    }
}