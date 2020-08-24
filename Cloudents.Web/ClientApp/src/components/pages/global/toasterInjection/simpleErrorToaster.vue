<template>
    <v-snackbar
        top
        color="error"
        :timeout="params.timeout || 3000"
        :value="true"
        @input="onCloseToaster"
    >
    <div class="text-center">
       {{params.text}}
    </div>
    </v-snackbar>
</template>


<script>
export default {
    name: 'simpleErrorToaster',
    props: {
        params: {
            type:Object,
            required:true
        }
    },
    methods: {
        onCloseToaster() {
            this.$store.commit('removeComponent',this.params.name)
        }
    },
}
</script>