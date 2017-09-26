<template>
    <div>
        <div inline-flex>
            <div v-for="v in values"><input @click="changeCallback" type="radio" :id="v.id" :value="v.id" v-model="model" :name="model" /><label :for="v.id">{{v.name}}</label></div>
        </div>
    </div>
</template>
<script>
    export default {
        props: { values: {}, model: {}, changeCallback: { type: Function } }
    }
</script>
