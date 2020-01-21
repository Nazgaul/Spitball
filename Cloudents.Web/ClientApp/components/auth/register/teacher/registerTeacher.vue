<template>
    <div id="registerTeacher">
        <component :is="teacherStep"></component>
    </div>
</template>

<script>
export default {
    name: 'registerTeacher',
}
</script>

<style lang="less">

</style>