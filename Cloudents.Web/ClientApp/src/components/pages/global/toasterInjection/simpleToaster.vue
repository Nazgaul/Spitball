<template>
    <v-snackbar
        top
        @input="onCloseToaster"
        :timeout="params.timeout || 3000"
        :value="true"
    >
    <div class="text-center">
       {{params.text}}
    </div>
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
            this.$store.commit('clearComponent')
        }
    },
}
</script>