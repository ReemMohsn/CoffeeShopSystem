// ... (Keep your navigation and toggle code as is) ...

let cards = document.querySelectorAll('.card');

// Iterate over each card element
cards.forEach(function (card) {
    // Get the icon element within the current card
    let icon = card.querySelector('.icon');

    // --- Robustness Check: Ensure icon exists ---
    if (!icon) {
        console.warn("Card found without an '.icon' element:", card);
        return; // Skip this card if no icon is found inside it
    }
    // --- End Robustness Check ---

    // Get the original and WE WILL DETERMINE the alternate image sources
    let originalSrc = icon.src;
    let alternateSrc = null; // Initialize alternateSrc

    // --- CORRECTED IF/ELSE IF CHAIN ---
    // Use more specific filenames like '.png' if possible to avoid partial matches
    if        (originalSrc.includes('D1.png')) {
        alternateSrc = originalSrc.replace('D1.png', 'D2.png');
    } else if (originalSrc.includes('D3.png')) {
        alternateSrc = originalSrc.replace('D3.png', 'D4.png');
    } else if (originalSrc.includes('D5.png')) {
        alternateSrc = originalSrc.replace('D5.png', 'D6.png');
    } else if (originalSrc.includes('D9.png')) {
        alternateSrc = originalSrc.replace('D9.png', 'D10.png');
    } else if (originalSrc.includes('D7.png')) { 
        alternateSrc = originalSrc.replace('D7.png', 'D8.png');
    } else if (originalSrc.includes('D11.png')) {
        alternateSrc = originalSrc.replace('D11.png', 'D12.png');
    } else if (originalSrc.includes('D15.png')) {
        alternateSrc = originalSrc.replace('D15.png', 'D16.png');
    } else {
        // Optional: Log if the icon doesn't match expected patterns
        console.log("Icon source doesn't match known D1-D12 pattern:", originalSrc);
        // You might want alternateSrc = originalSrc; here if no match
        // This prevents errors in the listeners if no alternate is found
        alternateSrc = originalSrc;
    }
    // --- END OF CORRECTED CHAIN ---


    // --- Make sure alternateSrc was successfully determined ---
    if (alternateSrc && alternateSrc !== originalSrc) {
        // Add event listeners for mouseenter and mouseleave
        card.addEventListener('mouseenter', function () {
            icon.src = alternateSrc;
        });

        card.addEventListener('mouseleave', function () {
            icon.src = originalSrc;
        });
    } else {
        // Optional warning if no valid alternate source was found for an icon
        // This might happen if the 'else' block above sets alternateSrc = originalSrc
        console.warn("No valid alternate source determined for:", originalSrc);
    }
});


// --- Keep your customerRows code as is, it looks logically correct ---
let customerRows = document.querySelectorAll('.customerRow');

// Iterate over each customerRow element
customerRows.forEach(function (customerRow) {
    // Get the profile image element within the current customerRow
    let profile = customerRow.querySelector('.profile');

    // --- Robustness Check: Ensure profile image exists ---
    if (!profile) {
        console.warn("customerRow found without a '.profile' element:", customerRow);
        return; // Skip this row
    }
    // --- End Robustness Check ---


    // Get the original and alternate image sources
    let originalSrc = profile.src;
    // Assuming alternate is always 'customer1' - adjust if needed
    let alternateSrc = originalSrc.replace('customer', 'customer1');

    // --- Check if replacement actually happened ---
    if (alternateSrc && alternateSrc !== originalSrc) {
        // Add event listeners for mouseenter and mouseleave
        customerRow.addEventListener('mouseenter', function () {
            profile.src = alternateSrc;
        });

        customerRow.addEventListener('mouseleave', function () {
            profile.src = originalSrc;
        });
    } else {
        console.warn("Could not determine alternate source for profile or source was unchanged:", originalSrc);
    }
});
