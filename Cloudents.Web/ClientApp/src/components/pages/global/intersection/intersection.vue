<template>
    <v-lazy
        v-model="isActive"
        :options="{
            threshold: .5
        }"
        transition="fade-transition"
    >
        <slot></slot>
    </v-lazy>
</template>

<script>
export default {
    name: 'intersection',
    data() {
        return {
            isActive: false
        }
    }
}
</script>