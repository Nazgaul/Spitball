<template>
    <div>
        <img src="https://dummyimage.com/600x400/000/fff" alt="something" />
        <span>something</span>
        <component v-bind:is="currentView">
            <!-- component changes when vm.currentView changes! -->
        </component>
    </div>
</template>


<script src="vertical.js">
</script>