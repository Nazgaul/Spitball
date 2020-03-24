<template>
    <v-snackbar
        absolute
        top
        @input="onCloseToaster"
        :timeout="params.timeout || 3000"
        :value="true"
    >
       {{params.text}}
    </v-snackbar>
</template>


<script>
export default {
    name: '',
    props: {
        params: {
            type:Object,
            required:true
        }
    },
    methods: {
        onCloseToaster() {
            this.$store.commit('clearToaster')
        }
    },
}
</script>