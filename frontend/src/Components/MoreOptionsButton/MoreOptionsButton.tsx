import React, { useState, useEffect } from "react";
import { FaEllipsisH } from "react-icons/fa"; 

const MoreOptionsButton = ({ onEdit, onRemove }: { onEdit: () => void; onRemove: () => void }) => {
  const [isMenuOpen, setIsMenuOpen] = useState(false);

  const toggleMenu = (e: React.MouseEvent) => {
    e.stopPropagation();
    setIsMenuOpen(!isMenuOpen);
  };

  const handleOutsideClick = (e: MouseEvent) => {
    if (!(e.target as HTMLElement).closest(".context-menu") && !(e.target as HTMLElement).closest("button")) {
      setIsMenuOpen(false);
    }
  };

  useEffect(() => {
    document.addEventListener("click", handleOutsideClick);
    return () => {
      document.removeEventListener("click", handleOutsideClick);
    };
  }, []);

  return (
    <div className="relative">
      <button
        onClick={toggleMenu}
        className="bg-[var(--color-options)] text-white px-4 py-4 flex items-center justify-center rounded-lg focus:outline-none"
        aria-label="More options"
      >
        <FaEllipsisH size={12} />
      </button>

      {isMenuOpen && (
        <div className="context-menu absolute right-0 mt-2 bg-white border rounded-md shadow-md z-10">
          <button
            onClick={onEdit}
            className="w-full text-left px-4 py-2 text-gray-700 hover:bg-gray-100"
          >
            Edit
          </button>
          <button
            onClick={onRemove}
            className="w-full text-left px-4 py-2 text-red-600 hover:bg-gray-100"
          >
            Remove
          </button>
        </div>
      )}
    </div>
  );
};

export default MoreOptionsButton;
