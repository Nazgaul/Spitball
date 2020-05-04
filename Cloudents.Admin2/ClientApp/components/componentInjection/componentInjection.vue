<template>
    <component :is="component.name" :params="component.params"></component>
</template>

<script>
import toasterError from '../toasters/toasterError.vue'

export default {
    name: "componentInjection",
    components: {
        toasterError
    },
    data() {
        return {
            component: {
                name: '',
                params: null
            },
        }
    },
    watch: {
        "$store.getters.getSnackbar.value": "injectToasterComponent"
    },
    methods: {
        injectToasterComponent(val) {
            let snack = this.$store.getters.getSnackbar

            this.component.name = snack.name
            this.component.params = snack.params
        }
    }
}
</script>