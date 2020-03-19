<template>
    <v-snackbar
        absolute
        top
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
    }
}
</script>