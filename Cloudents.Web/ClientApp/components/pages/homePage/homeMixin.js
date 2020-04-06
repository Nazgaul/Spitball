import * as routeNames from '../../../routes/routeNames'

export default {

    methods: {
        startLearn() {
            this.$router.push({name: routeNames.Learning})
        }
    }

}