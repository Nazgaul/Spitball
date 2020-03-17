<template>
    <v-snackbar
        absolute
        top
        :timeout="toasterObj.timeout || 3000"
        :value="true"
    >
       {{toasterObj.text}}
    </v-snackbar>
</template>


<script>
export default {
    name: '',
    props: {
        toasterObj: {
            type: Object,
            required: true
        }
    }
}
</script>