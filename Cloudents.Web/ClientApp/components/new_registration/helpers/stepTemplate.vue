<template functional>
    <div class="step-wrapp-one">
        <div class="text-block-top">
        <slot name="step-text"></slot>
        </div>
        <div class="step-wrapper">
             <div class="side-wrapper image">
                <slot name="step-image"></slot>
            </div>
            <div class="side-wrapper data">
                <slot name="step-data"></slot>
            </div>           
        </div>
    </div>
</template>
